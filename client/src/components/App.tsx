import {WsClientProvider} from 'ws-request-hook';
import MockMqttDevice from "./MockMqttDevice.tsx";
import AdminDashboard from "./AdminDashboard.tsx";
import {useEffect, useState} from "react";
const baseUrl = import.meta.env.VITE_API_BASE_URL
const prod = import.meta.env.PROD

export default function App() {
    
    const [url, setUrl] = useState<string | undefined>(undefined)
    useEffect(() => {
        const finalUrl = prod ? 'wss://' + baseUrl + '?id=' + crypto.randomUUID() : 'ws://' + baseUrl + '?id=' + crypto.randomUUID();
setUrl(finalUrl);
    }, []);
    
    return (<>

        {
            url &&
        <WsClientProvider url={url}>

            <div className="flex flex-col"><h1>Everything up here is the admin web panel ONLY communicating with C#
                backer (which then communicates with the broker, for instance)
            </h1>
                <div className="h-[45vh]">
                    <AdminDashboard />
                </div>
                <hr/>
                <h1>Everything down here is MQTT devices ONLY communicating with broker</h1>

                <div className="h-[45vh] flex flex-row ">
                    {
                        ["A", "B"].map((item) => (
                            <MockMqttDevice id={item} key={item}/>

                        ))
                    }                        </div>

            </div>
        </WsClientProvider>
        }
    </>)
}