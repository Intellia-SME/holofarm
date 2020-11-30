import sys
import time
from kafka import KafkaConsumer
import json
import os
from random import seed
from random import randint

topic = "DroneTopic"

# Start up producer
#consumer = KafkaConsumer(bootstrap_servers='localhost:9092')

consumer = KafkaConsumer('id',
    bootstrap_servers='eagle5.di.uoa.gr:9092',
    auto_offset_reset='latest',
    consumer_timeout_ms=1000) #,
    # value_deserializer = json.loads)

consumer.subscribe(topic)

for msg in consumer:
    value_to_process = msg.value
    print(value_to_process)
    print(type(value_to_process))

# while True:
#     try:
#         msg = consumer.poll()

#     except SerializerError as e:
#         print("Message deserialization failed for")
#         break

#     if msg is None:
#         continue

#     print(msg.keys())
#     # print(msg)
#     # break
    #print(json.dumps(msg.value()))

# consumer.close()