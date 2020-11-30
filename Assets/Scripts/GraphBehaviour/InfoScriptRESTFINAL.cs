using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class InfoScriptRESTFINAL : MonoBehaviour
{

    [Header("Be sure that both scripts have the same attributes!")]
    [Header("Graph Data Detail")]

    [SerializeField, Tooltip("Phainomenon Variabe name: ")]
    private string variable = "Info";
    public String Variable_Name
    {
        get { return variable; }
        set { variable = value; }
    }

    [Header("Kafka REST Proxy Configuration")]
    [SerializeField, Tooltip("Set the Host URI: ")]
    private string HOST_WEB_URL = "http://eagle5.di.uoa.gr:8082/";
    public String Kafka_REST_Host_URI
    {
        get { return HOST_WEB_URL; }
        set { HOST_WEB_URL = value; }
    }

    //"consumers/drone_consumer/instances/drone_consumer_instance/records"
    [SerializeField, Tooltip("Set the Kafka REST Proxy cosumer name: ")]
    private string consumer = "consumer";
    public String Kafka_Consumer
    {
        get { return consumer; }
        set { consumer = value; }
    }
    [SerializeField, Tooltip("Set the Kafka REST Proxy cosumer instance name: ")]
    private string c_instance = "consumer_instance";
    public String Kafka_Consumer_Instance
    {
        get { return c_instance; }
        set { c_instance = value; }
    }
    
    public Text batteryStatus, batteryLevel, connectivity, alerts, timestamp;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetKafkaMessages());
    }

        private IEnumerator GetKafkaMessages()
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

                JSONObject infoMsg = null;
                foreach(JSONObject obj in responseJson.list){
                    string topic = obj.GetField("topic").ToString();
                    topic.Remove(0);
                    topic.Remove(topic.Length-1);
                    Debug.Log(topic);
                    //InfoTopic
                    if(topic.Equals("\"InfoTopic\"")){
                        Debug.Log(obj.GetField("topic"));
                        JSONObject temp = obj;
                        infoMsg = temp.GetField("value");
                    }
			    }

                if(infoMsg != null){
                    Debug.Log(infoMsg);
                    this.connectivity.text = infoMsg.GetField("connectivity").ToString();
                    this.alerts.text = infoMsg.GetField("alert").ToString();
                }
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //all values must be handleded here for live
        //position.text = GameObject.Find("UI").GetComponent<RectTransform>().position.ToString();
        batteryStatus.text = SystemInfo.batteryStatus.ToString();
        float level = SystemInfo.batteryLevel * 100;
        if(level < 50.0f)
        {
            batteryLevel.color = Color.yellow;
        }else if(level < 30.0f)
        {
            batteryLevel.color = Color.red;
        }
        else
        {
            batteryLevel.color = Color.green;
        }
        batteryLevel.text = level.ToString("F2") + "%";
        timestamp.text = DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
        //alerts.text = "New Incoming Data";
    }

    private void Awake()
    {
        /*
        Utilities.FunctionPeriodic.Create(() => {

            int choice = UnityEngine.Random.Range(0, 4);
            switch (choice)
            {
                case 1:
                    connectivity.text = "Cellular Network";
                    break;
                case 2:
                    connectivity.text = "WiFi Network";
                    break;
                default:
                    connectivity.text = "Satelite Connection";
                    break;
            }
        
        }, 10.0f);
        */
    }
}
