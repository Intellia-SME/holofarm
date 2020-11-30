from confluent_kafka import KafkaError
from confluent_kafka.avro import AvroConsumer
from confluent_kafka.avro.serializer import SerializerError
import json, ast
import argparse

# Define argparser
parser = argparse.ArgumentParser(description="Avro Kafka Consumer")
parser.add_argument('--schema_registry_host', default=None, help="schema registry host")
parser.add_argument('--topic', type=str, default=None, help="topic to consume")

args = parser.parse_args()

c = AvroConsumer({
    'bootstrap.servers': "{}:9092".format(args.schema_registry_host),
    'group.id': 'groupid',
    'schema.registry.url': "http://{}:8081".format(args.schema_registry_host)})

c.subscribe(["{}".format(args.topic)])

while True:
    try:
        msg = c.poll(10)

    except SerializerError as e:
        print("Message deserialization failed for {}: {}".format(msg, e))
        break

    if msg is None:
        continue

    if msg.error():
        if msg.error().code() == KafkaError._PARTITION_EOF:
            continue
        else:
            print(msg.error())
            break

    #print(msg.value())
    print(ast.literal_eval(json.dumps(msg.value()))) 

c.close()
