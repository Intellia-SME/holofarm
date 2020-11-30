using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class KafkaFINALRESTProxy : MonoBehaviour
{

    [Header("Kafka REST Proxy Configuration")]
    [SerializeField, Tooltip("Set the Host URI: ")]
    private string HOST_WEB_URL = "http://eagle5.di.uoa.gr:8082/";
    public String Kafka_REST_Host_URI
    {
        get { return HOST_WEB_URL; }
        set { HOST_WEB_URL = value; }
    }

    [SerializeField, Tooltip("Set the topics: ")]
    private string[] kafka_topics = { "DroneTopic", "VectorTopic", "InfoTopic" };
    public String[] Kafka_Topic
    {
        get { return kafka_topics; }
        set { kafka_topics = value; }
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

    /*
    // Start is called before the first frame update
    private Graph graph = null;
    private Graph_Dynamic graph_Dynamic = null;
    private InfoScript infoScript = null;

    //for Graph
    public string key;
    public int value;
    //for Graph_Dynamic
    public List<string> keyList;
    public List<int> valueList = new List<int>() {1,1,1,1};
    //for InfoScript
    public string connectivity, alertMsg;
    */

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
                Debug.LogError(request.error);
                Debug.LogError(request.responseCode);
                //THIS IS AN EXCEPTION THAT DOES NT SIGNIFY AN ERROR
                if(request.responseCode == 409)
                {
                    Debug.LogWarning(request.responseCode);
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
        public string[] topics = { "DroneTopic", "VectorTopic", "InfoTopic" };
    }

    private IEnumerator Subscribe()
    {
        while (!subscribed)
        {
            //String[] topics_array = {"DroneTopic"};
            //Debug.Log(topics_array[0]);
            Topics mytopics = new Topics();
            mytopics.topics = Kafka_Topic;
            Debug.Log(mytopics.topics[0] + mytopics.topics[1] + mytopics.topics[2]);
            string jsontopic = JsonUtility.ToJson(mytopics, true);
            Debug.Log(jsontopic);
            
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
                Debug.LogError(request.error);
                //System.Diagnostics.Debug.Write(request.error);
            }
            else
            {
                subscribed = true;
                System.Diagnostics.Debug.Write("Subscription complete!");
                System.Diagnostics.Debug.Write(request.responseCode);
                Debug.Log("Subscription complete!");
                Debug.Log(request.responseCode);
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
        string targetURI = HOST_WEB_URL + "consumers/"+consumer+"/instances/"+c_instance+"/records";

        while (true)
        {
            var request = new UnityWebRequest(targetURI, "GET");
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Accept", "application/vnd.kafka.json.v2+json");
            request.SendWebRequest();

            yield return new WaitUntil(() => request.isDone);

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                System.Diagnostics.Debug.Write(request.error);
            }
            else
            {
                //Debug.Log("GET complete!");
                //Debug.Log(request.responseCode);
                //Debug.Log(request.downloadHandler.data.ToString());
                string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
                var data = request.downloadHandler.text;
                //String response = request.downloadHandler.text;
                //Debug.LogError(data);
                //response.Remove(0);
                //response.Remove(response.Length-2);
                
                //Debug.Log(response.Length);
                if(response.Length > 500)
                {
                    continue;
                }

                if(response.Length == 0)
                {
                    continue;
                }


                //Debug.Log(response);
                //Response responseData = JsonUtility.FromJsonOverwrite(response);
                
                //this is automatically set to an array
                JSONObject responseJson = new JSONObject(response);
                //accessData(responseJson);
                if (responseJson == null)
                {
                    continue;
                }
                //accessData(responseJson);

                JSONObject droneMsg = null;
                JSONObject vectorMsg = null;
                JSONObject infoMsg = null;
                foreach(JSONObject obj in responseJson.list){
                    string topic = obj.GetField("topic").ToString();
                    topic.Remove(0);
                    topic.Remove(topic.Length-1);
                    Debug.Log(topic);

                    //DroneTopic
				    if(topic.Equals("\"DroneTopic\"")){
                        Debug.Log(obj.GetField("topic"));
                        droneMsg = obj;
                    }
                    //VectorTopic
                    if(topic.Equals("\"VectorTopic\"")){
                        Debug.Log(obj.GetField("topic"));
                        vectorMsg = obj;
                    }
                    //InfoTopic
                    if(topic.Equals("\"InfoTopic\"")){
                        Debug.Log(obj.GetField("topic"));
                        infoMsg = obj;
                    }
			    }

                /*
                if(droneMsg != null){
                    Debug.Log(droneMsg);
                    this.key = droneMsg.keys[0];
                    string val = droneMsg.GetField(key).ToString();
                    int value = int.Parse(val);
                    this.value = value;
                }
                if(vectorMsg != null){
                    Debug.Log(vectorMsg);
                    this.keyList = vectorMsg.keys;
                    for(int i=0; i<keyList.Count; i++){
                        string val = vectorMsg.GetField(keyList[i]).ToString();
                        this.valueList[i] = int.Parse(val);
                    }
                }
                if(infoMsg != null){
                    Debug.Log(infoMsg);
                    this.connectivity = infoMsg.GetField("connectivity").ToString();
                    this.alertMsg = infoMsg.GetField("alert").ToString();
                }
                */
            }
        }
    }

    public void UpdateGraph(int value){
        
    }

    public void UpdateDynamicGraph(){

    }

    public void UpdateInfoGraph(){

    }

    void accessData(JSONObject obj){
	switch(obj.type){
		case JSONObject.Type.OBJECT:
			for(int i = 0; i < obj.list.Count; i++){
                Debug.Log("inside keys");
				string key = (string)obj.keys[i];
				JSONObject j = (JSONObject)obj.list[i];
				Debug.Log(key);
				accessData(j);
			}
			break;
		case JSONObject.Type.ARRAY:
        Debug.Log("for array");
			foreach(JSONObject j in obj.list){
				accessData(j);
			}
			break;
		case JSONObject.Type.STRING:
			Debug.Log(obj.str);
			break;
		case JSONObject.Type.NUMBER:
			Debug.Log(obj.n);
			break;
		case JSONObject.Type.BOOL:
			Debug.Log(obj.b);
			break;
		case JSONObject.Type.NULL:
			Debug.Log("NULL");
			break;
		
	}
}

    private  void DroneTopic(JSONObject message){
        string key = message.keys[0];
        Debug.Log("received :" +key+message.GetField(key));
    }

    private void VectorTopic(JSONObject message){
        List<string> keys = message.keys;
         Debug.Log("received vector");
        for(int i=0; i<keys.Count; i++){
           Debug.Log(keys[i] + " " + message.GetField(keys[i]));
        }
    }

    private void InfoTopic(JSONObject message){
        List<string> keys = message.keys;
         Debug.Log("received info");
        for(int i=0; i<keys.Count; i++){
           Debug.Log(keys[i] + " " + message.GetField(keys[i]));
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
