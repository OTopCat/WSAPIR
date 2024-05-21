import openai
import os
from git import Repo
import glob

openai.api_key = os.getenv("OPENAI_API_KEY")

def get_code_from_repo(repo, file_path):
    try:
        file_content = repo.git.show(f'HEAD:{file_path}')
        return file_content
    except Exception as e:
        print(f"Error getting file from repo: {e}")
        return None

def generate_unit_tests(code, class_name):
    response = openai.Completion.create(
        engine="davinci-codex",
        prompt=f"Generate C# unit tests for the following ASP.NET Web API class:\n\n{code}",
        max_tokens=500,
        temperature=0.5,
        n=1,
        stop=None
    )
    return response.choices[0].text.strip()

def save_tests_to_repo(repo_path, test_file_path, tests):
    with open(os.path.join(repo_path, test_file_path), 'w') as test_file:
        test_file.write(tests)
    repo = Repo(repo_path)
    repo.index.add([test_file_path])
    repo.index.commit("Add generated unit tests")
    repo.git.push()

def get_class_name(file_path):
    with open(file_path, 'r') as file:
        for line in file:
            if line.strip().startswith("public class"):
                return line.strip().split()[2]

if __name__ == "__main__":
    repo_path = os.getenv("REPO_PATH")
    src_dir = os.getenv("SRC_DIR")
    test_dir = os.getenv("TEST_DIR")

    repo = Repo(repo_path)

    # Get all .cs files in the source directory that exist in the latest commit
    cs_files = [file for file in glob.glob(os.path.join(repo_path, src_dir, '**', '*.cs'), recursive=True) if repo.git.ls_files(file)]

    for cs_file in cs_files:
        class_name = get_class_name(cs_file)
        if class_name:
            code = get_code_from_repo(repo, cs_file)
            if code:
                tests = generate_unit_tests(code, class_name)
                test_file_path = os.path.join(test_dir, f"Test{class_name}.cs")
                save_tests_to_repo(repo_path, test_file_path, tests)
