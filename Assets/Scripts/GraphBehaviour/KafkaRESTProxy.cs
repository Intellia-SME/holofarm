using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class KafkaRESTProxy : MonoBehaviour
{

    [Header("Kafka REST Proxy Configuration")]
    [SerializeField, Tooltip("Set the Host URI: ")]
    private string HOST_WEB_URL = "http://eagle5.di.uoa.gr:8082/";
    public String Kafka_REST_Host_URI
    {
        get { return HOST_WEB_URL; }
        set { HOST_WEB_URL = value; }
    }

    [SerializeField, Tooltip("Set the Host URI: ")]
    private string kafka_topic = "DroneTopic";
    public String Kafka_Topic
    {
        get { return kafka_topic; }
        set { kafka_topic = value; }
    }

    //"consumers/drone_consumer/instances/drone_consumer_instance/records"
    [SerializeField, Tooltip("Set the Kafka REST Proxy cosumer name: ")]
    private string consumer = "drone_consumer";
    public String Kafka_Consumer
    {
        get { return consumer; }
        set { consumer = value; }
    }
    [SerializeField, Tooltip("Set the Kafka REST Proxy cosumer instance name: ")]
    private string c_instance = "drone_consumer_instance";
    public String Kafka_Consumer_Instance
    {
        get { return c_instance; }
        set { c_instance = value; }
    }

    private bool created = false;
    private bool subscribed = false;


    // Start is called before the first frame update
    void Start()
    {
        this.configureKafkaProxy();
    }

    private void configureKafkaProxy()
    {
        StartCoroutine(CreateRESTConsumer());
        StartCoroutine(Subscribe());
        //StartCoroutine(GetSomething());
    }

    private IEnumerator CreateRESTConsumer()
    {

        while (!created)
        {
            JSONObject config = new JSONObject();
            config.SetField("name", c_instance);
            config.SetField("format", "json");
            config.SetField("auto.offset.reset", "latest");
            config.SetField("auto.commit.enable", "false");
            //config["delete.retention.ms"] = ""
            //config["flush.messages"] = "1";

            string targetURI = HOST_WEB_URL + "consumers/" + consumer + "/";

            var request = new UnityWebRequest(targetURI, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(config.ToString());
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/vnd.kafka.json.v2+json");
            request.SetRequestHeader("Accept", "application/vnd.kafka.v2+json");
            request.SendWebRequest();

            yield return new WaitUntil(() => request.isDone);

            if (request.isNetworkError || request.isHttpError)
            {
                //Debug.LogError(request.error);
                //Debug.LogError(request.responseCode);
                //THIS IS AN EXCEPTION THAT DOES NT SIGNIFY AN ERROR
                if(request.responseCode == 409)
                {
                    //Debug.LogWarning(request.responseCode);
                    created = true;
                }
            }
            else
            {
                created = true;
                //Debug.Log("Form upload complete!");
                //Debug.Log(request.responseCode);
            }
        }
        yield return null;
    }

    //curl -X POST -H "Content-Type: application/vnd.kafka.v2+json" --data '{"topics":["jsontest"]}' 
    //http://localhost:8082/consumers/my_json_consumer/instances/my_consumer_instance/subscription

    [Serializable]
    public class Topics
    {
        public string[] topics = { "DroneTopic" };
    }

    private IEnumerator Subscribe()
    {
        while (!subscribed)
        {
            //String[] topics_array = {"DroneTopic"};
            //Debug.Log(topics_array[0]);
            Topics mytopic = new Topics();
            mytopic.topics = new string[] { Kafka_Topic };
            string jsontopic = JsonUtility.ToJson(mytopic, true);
            //Debug.Log(jsontopic);
            
            string targetURI = HOST_WEB_URL + "consumers/"+ consumer + "/instances/"+ c_instance 
                                +"/subscription/";
            //Debug.Log(targetURI);
            var request = new UnityWebRequest(targetURI, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsontopic);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/vnd.kafka.v2+json");
            request.SendWebRequest();

            yield return new WaitUntil(() => request.isDone);

            if (request.isNetworkError || request.isHttpError)
            {
                //Debug.LogError(request.error);
                //System.Diagnostics.Debug.Write(request.error);
            }
            else
            {
                subscribed = true;
                System.Diagnostics.Debug.Write("Subscription complete!");
                System.Diagnostics.Debug.Write(request.responseCode);
                //Debug.Log("Subscription complete!");
                //Debug.Log(request.responseCode);
            }
        }
        yield return null;
    }

    //curl -X GET -H "Accept: application/vnd.kafka.json.v2+json" \
    //http://localhost:8082/consumers/my_json_consumer/instances/my_consumer_instance/records
    //[{"topic":"DroneTopic","key":null,"value":{"altitude":60},"partition":0,"offset":73145}]

    /*
    [System.Serializable]
    public class Response
    {
        public string topic { get; set; }
        public string key { get; set; }
        public JObject value { get; set; }
        public string partition { get; set; }
        public int offset { get; set; }
    }

    [System.Serializable]
    public class Message
    {
        public int altitude { get; set; }
    }
    */

    private IEnumerator GetSomething()
    {
        string targetURI = HOST_WEB_URL + "consumers/drone_consumer/instances/drone_consumer_instance/records";

        while (true)
        {
            var request = new UnityWebRequest(targetURI, "GET");
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Accept", "application/vnd.kafka.json.v2+json");
            request.SendWebRequest();

            yield return new WaitUntil(() => request.isDone);

            if (request.isNetworkError || request.isHttpError)
            {
                //Debug.LogError(request.error);
                System.Diagnostics.Debug.Write(request.error);
            }
            else
            {
                //Debug.Log("GET complete!");
                //Debug.Log(request.responseCode);
                //Debug.Log(request.downloadHandler.data.ToString());
                //string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
                var data = request.downloadHandler.text;
                String response = data.Substring(1, data.Length - 2);
                //Debug.LogError(data);
                //response.Remove(0);
                //response.Remove(response.Length-2);
                //Debug.Log(response);
                //Debug.Log(response.Length);
                if(response.Length > 100)
                {
                    continue;
                }

                //var jsonObject = JsonConvert.DeserializeObject<Response>(response);
                //"{\"scores\": " + FromServer + "}"
                //responseData = JsonUtility.FromJson<Response>("{\"[{\": " + response + "}]");

                //JsonUtility.FromJson<Response>(jsonObject);


                //Debug.LogWarning(responseData.ToString());

                if(response.Length == 0)
                {
                    continue;
                }

                //Response responseData = JsonUtility.FromJsonOverwrite(response);
                JSONObject responseJson = new JSONObject(response);
                JSONObject message = null; ;


                if (responseJson != null)
                {
                    message = responseJson.GetField("value");

                }
                Debug.LogWarning(responseJson.ToString());

                string key = "";
                if (message != null)
                {
                    //Debug.LogWarning(message.ToString());
                    key = message.keys[0];
                    //Debug.Log("the val is : " + key + " " + message.GetField(key));
                }

                //EXCEPTIONALLY WRONG CODE FOR HOLOLENS APP!!!!!!!!!
                //THE DESERIALIZER CRASHES
                //STAY AWAY OF THESE Newtonsoft libraries!!!!!
                /*
                var json = JObject.Parse(response);
                Debug.Log(json);
                var jsonObject = JsonConvert.DeserializeObject<Response>(json.ToString());
                Debug.Log(jsonObject);
                
                Debug.Log(jsonObject.topic);
                Debug.Log(jsonObject.key);
                Debug.Log(jsonObject.offset);
                Debug.Log(jsonObject.partition);
                Debug.Log(jsonObject.value);

                Debug.Log(jsonObject.value);



                if (jsonObject.value != null)
                {

                    Message message = JsonConvert.DeserializeObject<Message>(jsonObject.value.ToString());
                    Debug.Log(message.altitude);
                }
                */
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
