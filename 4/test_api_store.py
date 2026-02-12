import pytest
import requests
import time
from fixtures import api_store_order_endpoint

@pytest.mark.parametrize('order', [
    {
        "id": 1239999,
        "petId": 1234,
        "quantity": 0,
        "shipDate": "2025-10-06T12:13:23.238Z",
        "status": "placed",
        "complete": True
    },
    {
        "id": 123590999,
        "petId": 1234999,
        "quantity": 123,
        "shipDate": "2025-10-06T12:13:30.238Z",
        "status": "available",
        "complete": True
    },
])
def test_store_api(api_store_order_endpoint, order):
    orderid = order["id"]

    req_post = requests.post(api_store_order_endpoint, json=order)
    assert req_post.status_code == 200

    req_get = requests.get(api_store_order_endpoint + f'/{orderid}')
    assert req_get.status_code == 200

    get_json = req_get.json()
    for key, val in order.items():
        if key != 'shipDate':
            assert key in get_json
            assert val == get_json[key]

    req_del = requests.delete(api_store_order_endpoint + f'/{orderid}')
    assert req_del.status_code == 200

    req_get = requests.get(api_store_order_endpoint + f'/{orderid}')
    assert req_get.status_code == 404

