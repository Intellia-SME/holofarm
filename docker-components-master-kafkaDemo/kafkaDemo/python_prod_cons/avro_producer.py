import time
import argparse
import socket
from confluent_kafka import avro
from confluent_kafka.avro import AvroProducer


# Define argparser
parser = argparse.ArgumentParser(description="Avro Kafka Generator")
parser.add_argument('--schema_registry_host', default=None, help="schema registry host")
parser.add_argument('--schema', type=str, default=None, help="schema to produce under")
parser.add_argument('--topic', type=str, default=None, help="topic to publish to")
parser.add_argument('--frequency', type=float, default=1.0, help="number of message per second")

#args = parser.parse_args()


class Messenger(object):

    def __init__(self):
        pass

    def get_message(self, timestamp):
        pass



class AttitudeMessenger(Messenger):

    def get_message(self, timestamp):
        phi = 0.0
        theta = 0.0
        psi = 0.0
        values = [phi, theta, psi]
        message = {"header": {"sourceSystem": "eu.rawfie.testUAV",
                              "sourceModule": "testUAV.guideSystem",
                              "time": timestamp},
                   "phi": phi,
                   "theta": theta,
                   "psi": psi}
        print("Message: {}".format(message))
        return message, values




def run(args, messenger):
    """Produce messages according to the specified Avro schema"""
    assert args.schema_registry_host is not None and args.schema is not None

    value_schema = avro.load(args.schema)
    conf = {'bootstrap.servers': "{}:9092".format(args.schema_registry_host),
            'schema.registry.url': "http://{}:8081".format(args.schema_registry_host)}
    avro_producer = AvroProducer(conf, default_value_schema=value_schema)

    while True:

        # Get current timestamp
        timestamp = int(time.time())

        # Assemble avro-formatted message filled with generated data
        message, values = messenger.get_message(timestamp)

        # Publish the message under the specified topic on the message bus
        avro_producer.produce(topic=args.topic, value=message)

        # Flush the buffer
        avro_producer.flush()

        # Wait a second
        time.sleep(1.0 / args.frequency)


if __name__ == "__main__":
    args_ = parser.parse_args()
    if args_.topic == "di_Attitude":
        messenger = AttitudeMessenger()
        run(args_, messenger)
    else:
        raise NotImplementedError(("extend the 'Messenger' abstract class "
                                   "to cover additional schemas and topics!"))
