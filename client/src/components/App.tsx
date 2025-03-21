import {WsClientProvider} from 'ws-request-hook';
import AdminDashboard from "./AdminDashboard.tsx";
import {useEffect, useState} from "react";
import SuperSimpleKahootCloneGui from "./SuperSimpleKahootCloneGui.tsx";
const baseUrl = import.meta.env.VITE_API_BASE_URL
const prod = import.meta.env.PROD

export const randomUid = crypto.randomUUID()

export default function App() {
    
    const [url, setUrl] = useState<string | undefined>(undefined)
    useEffect(() => {
        const finalUrl = prod ? 'wss://' + baseUrl + '?id=' + randomUid : 'ws://' + baseUrl + '?id=' + randomUid;
setUrl(finalUrl);
    }, [prod, baseUrl]);
    
    return (<>

        {
            url &&
        <WsClientProvider url={url}>

            <div className="flex flex-col"><h1>Everything up here is the admin web panel ONLY communicating with C#
                backer (which then communicates with the broker, for instance)
            </h1>
                <div>
                    <AdminDashboard />
                    { !prod && <SuperSimpleKahootCloneGui /> }
                </div>

            </div>
        </WsClientProvider>
        }
    </>)
}