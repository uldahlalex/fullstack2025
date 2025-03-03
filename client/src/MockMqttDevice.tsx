import mqtt from "mqtt";
import { useState, useEffect } from "react";

export interface MqttCredentials {
    username: string;
    password: string;
}

export default function MockMqttDevice() {
    const [credentials, setCredentials] = useState<MqttCredentials>({
        username: '',
        password: ''
    });
    const [client, setClient] = useState<mqtt.MqttClient | null>(null);
    const [status, setStatus] = useState('Disconnected');

    const connectToBroker = () => {
        try {
            const mqttClient = mqtt.connect('wss://ae78af72ecf943f6ae2cfbd6a4a4cf44.s1.eu.hivemq.cloud:8884/mqtt', {
                username: credentials.username,
                password: credentials.password,
                protocol: 'wss',
                rejectUnauthorized: false
            });

            mqttClient.on('connect', () => {
                setStatus('Connected');
                console.log('Connected to MQTT broker');

                mqttClient.subscribe('devices/A', (err) => {
                    if (err) {
                        console.error('Subscribe error:', err);
                    } else {
                        console.log('Subscribed to devices/A');
                    }
                });
            });

            mqttClient.on('message', (topic, message) => {
                console.log('Received message:', topic, message.toString());
            });

            mqttClient.on('error', (err) => {
                console.error('MQTT Error:', err);
                setStatus('Error: ' + err.message);
            });

            setClient(mqttClient);
        } catch (err) {
            console.error('Connection error:', err);
            setStatus('Connection failed');
        }
    };

    const publishMessage = () => {
        if (client?.connected) {
            client.publish('devices/A', JSON.stringify({ myKey: "hey" }), (err) => {
                if (err) {
                    console.error('Publish error:', err);
                } else {
                    console.log('Message published');
                }
            });
        }
    };

    useEffect(() => {
        return () => {
            if (client) {
                client.end();
            }
        };
    }, [client]);

    return (
        <div className="p-4 space-y-4">
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

            <div className="space-x-2">
                <button
                    onClick={connectToBroker}
                    className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
                    disabled={client?.connected}
                >
                    Connect
                </button>

                <button
                    onClick={publishMessage}
                    className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600"
                    disabled={!client?.connected}
                >
                    Publish Test Message
                </button>
            </div>
        </div>
    );
}