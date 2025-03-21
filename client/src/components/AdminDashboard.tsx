import {useWsClient} from "ws-request-hook";
import {useEffect, useState} from "react";
import {Bar, BarChart, CartesianGrid, Legend, Rectangle, ResponsiveContainer, Tooltip, XAxis, YAxis} from "recharts";
import {
    ClientWantsToEnterDashboardDto, DeviceClient, Devicelog, ServerAddsAdminToDashboard,
    ServerSendsMetricToAdmin,
    StringConstants,
} from "../generated-client.ts";
const baseUrl = import.meta.env.VITE_API_BASE_URL 
const prod = import.meta.env.PROD;

const httpClient = new DeviceClient(prod ? "https://" : "http://"+baseUrl);

export default function AdminDashboard() {

    const {onMessage, readyState, sendRequest} = useWsClient()
    const [metric, setMetrics] = useState<Devicelog[]>([])

    useEffect(() => {
        if (readyState!=1)
            return;
        sendRequest<ClientWantsToEnterDashboardDto, ServerAddsAdminToDashboard>({eventType: StringConstants.ClientWantsToEnterDashboardDto}, StringConstants.ServerAddsAdminToDashboard).then(response => {
            setMetrics(response.devicelogs || [])
        });

        const unsub = onMessage<ServerSendsMetricToAdmin>(StringConstants.ServerSendsMetricToAdmin, (dto) =>  {
            console.log(dto)
            setMetrics(dto.metrics || []);
        })

        return () => {
            unsub();
        }
    }, [readyState]);
    

    return(<>
        

        <ResponsiveContainer width="100%" height={400}>
            <BarChart
                width={500}
                height={300}
                data={metric}
                margin={{
                    top: 5,
                    right: 30,
                    left: 20,
                    bottom: 5,
                }}
            >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="formattedTime" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="value" name="Temperature" fill="#8884d8" activeBar={<Rectangle fill="pink" stroke="blue" />} />
            </BarChart>
        </ResponsiveContainer>
        {/*Example: Communicating with API to dispatch message to edge device to change preferences*/}
        {/*<button className="btn" onClick={() => {*/}
        {/*    const dto: AdminWantsToChangePreferencesForDeviceDto =*/}
        {/*        {*/}
        {/*            intervalMilliseconds: millis,*/}
        {/*            unit: "Celcius", // yes this is hardcoded*/}
        {/*            deviceId: "A" //yes, this is hardcoded*/}
        {/*        }*/}
        {/*    httpClient.adminWantsToChangePreferencesForDevice(dto).then(resp => {*/}
        {/*        toast('API sent preference change to edge devices')*/}
        {/*    })*/}
        {/*}}>Change preferences to send every {millis} milliseconds</button>*/}
        {/*<button className="btn btn-warning" onClick={() => {*/}
        {/*    httpClient.adminWantsToClearData().then(result => {*/}
        {/*        setMetrics(result);*/}
        {/*    })*/}
        {/*}}>Clear all data</button>*/}

    </>)
}