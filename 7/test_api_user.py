import allure
import pytest
import requests
from fixtures import api_user_endpoint
from models import User

@allure.parent_suite('API')
@allure.suite('User')
@allure.sub_suite('Functional')
@allure.tag('User', 'Functional')
@allure.feature('User API')
@allure.story('Functional')
@allure.title('Test full user lifecycle: create, get, update, delete')
@pytest.mark.xfail(reason="API is known to be unstable")
@pytest.mark.parametrize('user,modified_user', [
    (User(id=657911, username="u9873460111", firstName="asdf", lastName="string",
          email="string", password="string", phone="string", userStatus=0),
     User(id=657911, username="u9873460111", firstName="fghh", lastName="fd",
          email="s84", password="s12", phone="s000", userStatus=2)),
])
def test_user_api_functional(api_user_endpoint, user: User, modified_user: User):
    username = user.username

    req_post = requests.post(api_user_endpoint, json=user.model_dump())
    assert req_post.status_code == 200

    req_get = requests.get(f'{api_user_endpoint}/{username}')
    assert req_get.status_code == 200
    retrieved_user = User.model_validate(req_get.json())
    assert retrieved_user == user

    req_put = requests.put(f'{api_user_endpoint}/{username}', json=modified_user.model_dump())
    assert req_put.status_code == 200

    req_get_modified = requests.get(f'{api_user_endpoint}/{username}')
    assert req_get_modified.status_code == 200
    retrieved_modified_user = User.model_validate(req_get_modified.json())
    assert retrieved_modified_user == modified_user

    req_del = requests.delete(f'{api_user_endpoint}/{username}')
    assert req_del.status_code == 200

    req_get_deleted = requests.get(f'{api_user_endpoint}/{username}')
    assert req_get_deleted.status_code == 404

@allure.parent_suite('API')
@allure.suite('User')
@allure.sub_suite('Negative')
@allure.tag('User', 'Negative', 'Get')
@allure.feature('User API')
@allure.story('Get Operations')
@allure.title('Test getting a non-existent user returns 404')
@pytest.mark.parametrize('username', ['nnasf123', '84230834', '---'])
def test_user_api_get_404(api_user_endpoint, username: str):
    req_get = requests.get(f'{api_user_endpoint}/{username}')
    assert req_get.status_code == 404

@allure.parent_suite('API')
@allure.suite('User')
@allure.sub_suite('Negative')
@allure.tag('User', 'Negative', 'Put')
@allure.feature('User API')
@allure.story('Put Operations')
@allure.title('Test updating a non-existent user returns 404')
@pytest.mark.parametrize('username', ['nonexistentuser1', 'nouserhere'])
def test_user_api_put_404(api_user_endpoint, username):
    user_data = User(id=12345, username=username, firstName="test", lastName="user",
                     email="test@example.com", password="password123", phone="1234567890", userStatus=1)
    req_put = requests.put(f"{api_user_endpoint}/{username}", json=user_data.model_dump())
    assert req_put.status_code == 200

@allure.parent_suite('API')
@allure.suite('User')
@allure.sub_suite('Negative')
@allure.tag('User', 'Negative', 'Delete')
@allure.feature('User API')
@allure.story('Delete Operations')
@allure.title('Test deleting a non-existent user returns 404')
@pytest.mark.parametrize('username', ['712471727412', '451472487129472918'])
def test_user_api_delete_404(api_user_endpoint, username):
    req_del = requests.delete(f"{api_user_endpoint}/{username}")
    assert req_del.status_code == 404




