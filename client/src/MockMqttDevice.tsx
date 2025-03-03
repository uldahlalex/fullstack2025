import mqtt from "mqtt";
import {useState, useEffect} from "react";

export interface MqttCredentials {
    username: string;
    password: string;
}

export interface TopicAndMessages {
    topic: string;
    messages: string[] 
}

interface Param {
    id: string;
}

export default function MockMqttDevice({id: id}: Param) {
    const [credentials, setCredentials] = useState<MqttCredentials>({
        username: '',
        password: ''
    });
    const [topicSubscriptions, setTopicSubscriptions] = useState<TopicAndMessages[]>([{
        topic: "device/"+id+"/changePreferences", messages: []
    }, {        topic: "device/"+id+"/feedback", messages: []
    }]);
    const [client, setClient] = useState<mqtt.MqttClient | null>(null);
    const [status, setStatus] = useState('Disconnected');

    const connectToBroker = () => {
        try {
            const mqttClient = mqtt.connect('wss://ae78af72ecf943f6ae2cfbd6a4a4cf44.s1.eu.hivemq.cloud:8884/mqtt', {
                username: credentials.username,
                password: credentials.password,
                protocol: 'wss',
                rejectUnauthorized: false,
                keepalive: 60,
                reconnectPeriod: 1000
            });

            mqttClient.on('connect', () => {
                setStatus('Connected');
                console.log('Connected to MQTT broker');

                topicSubscriptions.forEach(topic => {
                    mqttClient.subscribe(topic.topic, (err) => {
                        if (err) {
                            console.error('Subscribe error:', err);
                        } else {
                            console.log('Subscribed to ' + topic);
                        }
                    });
                });
            });

            mqttClient.on('message', (topic, message) => {
                console.log('Received message:', topic, message.toString());
                const duplicate = [...topicSubscriptions];
                const id= duplicate.findIndex((item) => item.topic === topic);
                if (id !== -1) {
                    duplicate[id].messages.push(message.toString());
                    setTopicSubscriptions([...duplicate]);
                }
            });

            mqttClient.on('error', (err) => {
                console.error('MQTT Error:', err);
                setStatus('Error: ' + err.message);
            });

            mqttClient.on('disconnect', () => {
                setStatus('Disconnected');
            });

            mqttClient.on('offline', () => {
                setStatus('Offline');
            });

            setClient(mqttClient);
        } catch (err) {
            console.error('Connection error:', err);
            setStatus('Connection failed');
        }
    };

  



    useEffect(() => {
        return () => {
            if (client) {
                client.end(true); 
                setClient(null);
                setStatus('Disconnected');
            }
        };
    }, []);

    return (
        <div className="p-4 space-y-4">
            <img style={
            {height: "200px"}
                
            } src="https://joy-it.net/files/files/Produkte/SBC-NodeMCU-ESP32/SBC-NodeMCU-ESP32-01.png"
                 />
            <div className="space-y-2">
                <div>Status: {status}</div>
                <input
                    placeholder="username"
                    type="text"
                    value={credentials.username}
                    onChange={e => setCredentials({...credentials, username: e.target.value})}
                    className="block w-full p-2 border rounded"
                />
                <input
                    placeholder="password"
                    type="password"
                    value={credentials.password}
                    onChange={e => setCredentials({...credentials, password: e.target.value})}
                    className="block w-full p-2 border rounded"
                />
            </div>

            <div className="space-y-4">
                <button
                    onClick={connectToBroker}
                    className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
                    disabled={client?.connected}
                >
                    Connect
                </button>

                <div className="space-y-2">
                    {topicSubscriptions.map(topic => (
                        <div key={topic.topic} className="flex items-center space-x-2">
                            <div>Subscribed to: {topic.topic}</div>
                            <span>{JSON.stringify(topic.messages)}</span>
                        </div>
                    ))}
                </div>

                {/*<div className="flex space-x-2">*/}
                {/*    <input*/}
                {/*        placeholder="topic"*/}
                {/*        value={newTopic}*/}
                {/*        onChange={e => setNewTopic(e.target.value)}*/}
                {/*        className="block p-2 border rounded"*/}
                {/*    />*/}
                {/*    <button*/}
                {/*        onClick={subscribeToTopic}*/}
                {/*        className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"*/}
                {/*        disabled={!client?.connected || !newTopic}*/}
                {/*    >*/}
                {/*        Subscribe to topic*/}
                {/*    </button>*/}
                {/*</div>*/}
            </div>
        </div>
    );
}