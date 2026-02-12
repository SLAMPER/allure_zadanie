import pytest
import requests
import allure
from fixtures import api_store_order_endpoint
from models import Order
from datetime import datetime

@allure.feature('Store API')
@allure.story('Functional')
@allure.title('Test full order lifecycle: create, get, delete')
@pytest.mark.xfail(reason="API is known to be unstable")
@pytest.mark.parametrize('order', [
    Order(id=1239999, petId=1234, quantity=0, shipDate=datetime.fromisoformat("2025-10-06T12:13:23.238Z"),
          status="placed", complete=True),
    Order(id=123590999, petId=1234999, quantity=123, shipDate=datetime.fromisoformat("2025-10-06T12:13:30.238Z"),
          status="available", complete=True),
])
def test_store_api_functional(api_store_order_endpoint, order: Order):
    orderid = order.id

    req_post = requests.post(api_store_order_endpoint, json=order.model_dump(by_alias=True))
    assert req_post.status_code == 200

    req_get = requests.get(f'{api_store_order_endpoint}/{orderid}')
    assert req_get.status_code == 200

    retrieved_order = Order.model_validate(req_get.json())

    assert retrieved_order.id == order.id
    assert retrieved_order.petId == order.petId
    assert retrieved_order.quantity == order.quantity
    assert retrieved_order.status == order.status
    assert retrieved_order.complete == order.complete

    req_del = requests.delete(f'{api_store_order_endpoint}/{orderid}')
    assert req_del.status_code == 200

    req_get_deleted = requests.get(f'{api_store_order_endpoint}/{orderid}')
    assert req_get_deleted.status_code == 404

@allure.feature('Store API')
@allure.story('Get Operations')
@allure.title('Test getting a non-existent order returns 404')
@pytest.mark.parametrize('orderid', ['99999', '101001', '84838'])
def test_store_api_get_404(api_store_order_endpoint, orderid):
    req_get = requests.get(f'{api_store_order_endpoint}/{orderid}')
    assert req_get.status_code == 404

@allure.feature('Store API')
@allure.story('Delete Operations')
@allure.title('Test deleting a non-existent order returns 404')
@pytest.mark.parametrize('orderid', ['987654321', '1122334455'])
def test_store_api_delete_404(api_store_order_endpoint, orderid):
    req_del = requests.delete(f"{api_store_order_endpoint}/{orderid}")
    assert req_del.status_code == 404







