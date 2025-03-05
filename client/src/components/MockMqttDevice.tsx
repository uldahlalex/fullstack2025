import mqtt from "mqtt";
import {useEffect, useState} from "react";
import toast from "react-hot-toast";
import {MetricEventDto} from "../generated-client.ts";

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

interface DevicePreferences {
    intervalMilliseconds: number,
    unit: string
}


export default function MockMqttDevice({id: id}: Param) {
    const [credentials, setCredentials] = useState<MqttCredentials>({
        username: '',
        password: ''
    });
    const [preferences, setPreferences] = useState<DevicePreferences>({
        intervalMilliseconds: 10000,
        unit: "Celcius",
    });
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
                mqttClient.subscribe('device/' + id + '/#', (err) => {
                    if (err) {
                        console.error('Subscribe error:', err);
                    }
                });
            });

            mqttClient.on('message', (topic, message) => {
                const command = topic.substring(topic.lastIndexOf('/') + 1);
                if (command === "changePreferences") {
                    console.log(command, message.toString());
                    const pref: DevicePreferences = JSON.parse(message.toString()) as DevicePreferences;
                    setPreferences(pref);
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

    const publishMetric = () => {
        if (!client?.connected) return;

        const topic = "device/" + id + "/metric";
        const metric: MetricEventDto = {
            value: Math.random() * 100,
            unit: preferences.unit,
            deviceId: id
        };
        console.log(metric)
        client.publish(topic, JSON.stringify(metric), (err) => {
            if (err) {
                console.error('Publish error:', err);
            } else {
                console.log(`Message published to ${topic}`);
                toast("Sending: " + JSON.stringify(metric));
            }
        });
    };

    /**
     * Publish every X seconds
     */
    useEffect(() => {
        if (!client?.connected) return;

        // Initial publish
        publishMetric();

        // Set up interval
        const interval = setInterval(publishMetric, preferences.intervalMilliseconds);

        // Cleanup
        return () => clearInterval(interval);
    }, [
        client?.connected,
        preferences.intervalMilliseconds,
        preferences.unit,
        id
    ]);

    return (
        <div className="p-4 space-y-4">
            <img style={
                {height: "200px"}

            } src="https://joy-it.net/files/files/Produkte/SBC-NodeMCU-ESP32/SBC-NodeMCU-ESP32-01.png"
            />
            <div className="space-y-2">
                <b>Connection to broker</b>
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
            </div>
        </div>
    );
}