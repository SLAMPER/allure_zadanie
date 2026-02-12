import pytest
import requests
import allure
from datetime import datetime
from fixtures import api_store_order_endpoint
from models import Order, OrderStatus

@allure.parent_suite('API')
@allure.suite('Store')
@allure.sub_suite('Post and Get')
@allure.tag('Store', 'Post', 'Get')
@allure.feature('Store API')
@allure.story('Post Operations')
@allure.title('Test creating and retrieving various valid orders')
@pytest.mark.xfail(reason="API is known to be unstable with frequent timeouts or 500 errors")
@pytest.mark.parametrize('order', [
    Order(id=778899, petId=10, quantity=1, shipDate=datetime(2025, 12, 1, 10, 30), status=OrderStatus.APPROVED, complete=True),
    Order(id=445566, petId=20, quantity=5, shipDate=datetime(2025, 11, 15, 18, 0), status=OrderStatus.DELIVERED, complete=False),
])
def test_create_and_get_valid_orders(api_store_order_endpoint, order):
    order_id = order.id

    post_response = requests.post(
        api_store_order_endpoint,
        data=order.model_dump_json(),
        headers={'Content-Type': 'application/json'}
    )
    assert post_response.status_code == 200
    posted_order = Order.model_validate(post_response.json())
    assert posted_order.id == order.id
    assert posted_order.status == order.status

    get_response = requests.get(f'{api_store_order_endpoint}/{order_id}')
    assert get_response.status_code == 200
    retrieved_order = Order.model_validate(get_response.json())

    assert retrieved_order.id == order.id
    assert retrieved_order.petId == order.petId
    assert retrieved_order.quantity == order.quantity
    assert retrieved_order.status == order.status
    assert retrieved_order.complete == order.complete

    delete_response = requests.delete(f'{api_store_order_endpoint}/{order_id}')
    assert delete_response.status_code == 200

@allure.parent_suite('API')
@allure.suite('Store')
@allure.sub_suite('Post Negative')
@allure.tag('Store', 'Negative', 'Post')
@allure.feature('Store API')
@allure.story('Post Operations')
@allure.title('Test creating an order with invalid data types')
@pytest.mark.parametrize('invalid_field, invalid_value', [
    ('id', 'id-int'),
    ('petId', '-int'),
    ('quantity', '-int')
])
def test_create_order_with_invalid_data(api_store_order_endpoint, invalid_field, invalid_value):
    base_order = Order(
        id=999111, petId=123, quantity=1,
        shipDate=datetime(2025, 10, 31, 23, 59),
        status=OrderStatus.PLACED, complete=True
    )
    order_json = base_order.model_dump()
    order_json['shipDate'] = order_json['shipDate'].isoformat() + "Z"
    order_json[invalid_field] = invalid_value

    post_response = requests.post(api_store_order_endpoint, json=order_json)
    assert post_response.status_code == 500

@allure.parent_suite('API')
@allure.suite('Store')
@allure.sub_suite('Delete')
@allure.tag('Store', 'Negative', 'Delete')
@allure.feature('Store API')
@allure.story('Delete Operations')
@allure.title('Test that deleting an order makes it irretrievable')
@pytest.mark.xfail(reason="API is known to be unstable")
def test_delete_order_and_verify_not_found(api_store_order_endpoint):
    order_data = Order(
        id=333222, petId=987, quantity=10,
        shipDate=datetime(2026, 1, 1, 0, 0),
        status=OrderStatus.PLACED, complete=False
    )
    order_id = order_data.id

    post_response = requests.post(
        api_store_order_endpoint,
        data=order_data.model_dump_json(),
        headers={'Content-Type': 'application/json'}
    )
    assert post_response.status_code == 200

    delete_response = requests.delete(f'{api_store_order_endpoint}/{order_id}')
    assert delete_response.status_code == 200

    get_response = requests.get(f'{api_store_order_endpoint}/{order_id}')
    assert get_response.status_code == 404





