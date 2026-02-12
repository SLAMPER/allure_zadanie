from pydantic import BaseModel, Field
from typing import Optional
from datetime import datetime
from enum import Enum

class User(BaseModel):
    id: int
    username: str
    firstName: str
    lastName: str
    email: str
    password: str
    phone: str
    userStatus: int = Field(..., description="User Status")

class OrderStatus(str, Enum):
    PLACED = "placed"
    APPROVED = "approved"
    DELIVERED = "delivered"
    AVAILABLE = "available"

class Order(BaseModel):
    id: Optional[int] = None
    petId: Optional[int] = None
    quantity: Optional[int] = None
    shipDate: Optional[datetime] = None
    status: Optional[OrderStatus] = Field(None, description="Order Status")
    complete: Optional[bool] = False



