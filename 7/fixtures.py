import pytest

BASE_URL_PETSTORE = 'http://localhost:5296/v2'


@pytest.fixture()
def api_store_order_endpoint():
    return f'{BASE_URL_PETSTORE}/store/order'


@pytest.fixture()
def api_user_endpoint():
    return f'{BASE_URL_PETSTORE}/user'

