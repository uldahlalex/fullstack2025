import {WsClientProvider} from 'ws-request-hook';
import MockMqttDevice from "../MockMqttDevice.tsx";
import {Suspense} from "react";

const baseUrl = import.meta.env.VITE_API_BASE_URL

export default function App() {
    return (<>
        <WsClientProvider url={baseUrl + '?id=' + crypto.randomUUID()}>

            <div className="flex flex-col">
                <div className="h-[45vh]">
                    <h1>Everything up here is the admin web panel ONLY communicating with C# backer (which then communicates with the broker, for instance)
                        </h1>
                </div>
                <hr />
                <div className="h-[45vh] flex flex-row ">
                    <h1>Everything down here is MQTT devices ONLY communicating with broker</h1>
                    {
                        ["A", "B"].map((item) => (
                            <MockMqttDevice id={item}/>

                        ))
                    }                        </div>

            </div>
        </WsClientProvider>
    </>)
}