import pytest
import requests
import time
from fixtures import api_user_endpoint

@pytest.mark.parametrize('user,modified_user', [
    ({
         "id": 657911,
         "username": "u9873460111",
         "firstName": "asdf",
         "lastName": "string",
         "email": "string",
         "password": "string",
         "phone": "string",
         "userStatus": 0
     },
     {
         "id": 657911,
         "username": "u9873460111",
         "firstName": "fghh",
         "lastName": "fd",
         "email": "s84",
         "password": "s12",
         "phone": "s000",
         "userStatus": 2
     }),
])
def test_user_api(api_user_endpoint, user: dict, modified_user: dict):
    username = user["username"]

    req_post = requests.post(api_user_endpoint, json=user)
    assert req_post.status_code == 200

    req_get = requests.get(api_user_endpoint + f'/{username}')
    assert req_get.status_code == 200

    get_json = req_get.json()
    for key, val in user.items():
        assert key in get_json
        assert val == get_json[key]

    req_put = requests.put(api_user_endpoint + f'/{username}', json=modified_user)
    assert req_put.status_code == 200

    req_get = requests.get(api_user_endpoint + f'/{username}')
    assert req_get.status_code == 200

    get_json = req_get.json()
    for key, val in modified_user.items():
        assert key in get_json
        assert val == get_json[key]

    req_del = requests.delete(api_user_endpoint + f'/{username}')
    assert req_del.status_code == 200

    req_get = requests.get(api_user_endpoint + f'/{username}')
    assert req_get.status_code == 404


