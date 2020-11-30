import sys
import time
from kafka import KafkaProducer
import json
import os
from random import seed
from random import randint

topic = "KafkaTopics"

# Start up producer
producer = KafkaProducer(bootstrap_servers='localhost:9092')

#conn.disconnect()
# seed random number generator
seed(1)
lower_bound = 0
upper_bound = 50
time2sleep = 10
topics = ("DroneTopic", "VectorTopic", "InfoTopic")
# generate some integers
while True:
	value = randint(50, 150)
	index = randint(0, len(connectivityStatus))
	data = {}
	data['altitude'] = value
	json_data = json.dumps(data)
	#json.dumps(data, default=json_util.default).encode('utf-8'))
	print(json_data)
	# Convert to bytes and send to kafka
	producer.send(topic, json.dumps(data).encode('utf-8'))
	time2sleep = randint(1,5)
	time.sleep(time2sleep)