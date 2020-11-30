import sys
import time
import json
import os
from random import seed
from random import randint
from kafka import KafkaProducer


topic = "DroneTopic"

# Start up producer
producer = KafkaProducer(bootstrap_servers='eagle5.di.uoa.gr:9092')

#conn.disconnect()
# seed random number generator
seed(1)
lower_bound = 0
upper_bound = 50
time2sleep = 1
# generate some integers
while True:
	value = randint(50, 150)
	#print(value)
	data = {}
	data['altitude'] = value
	json_data = json.dumps(data)
	#json.dumps(data, default=json_util.default).encode('utf-8'))
	print(json_data)
	# Convert to bytes and send to kafka
	producer.send(topic, json.dumps(data).encode('utf-8'))
	#time2sleep = randint(10,50)
	time.sleep(1)