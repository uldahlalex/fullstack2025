import {WsClientProvider} from 'ws-request-hook';
import MockMqttDevice from "../MockMqttDevice.tsx";

const baseUrl = import.meta.env.VITE_API_BASE_URL

export default function App() {
    return (<>
        <WsClientProvider url={baseUrl + '?id=' + crypto.randomUUID()}>

            <MockMqttDevice />
        </WsClientProvider>
    </>)
}