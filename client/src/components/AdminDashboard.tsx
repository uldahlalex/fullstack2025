import {useWsClient} from "ws-request-hook";
import {useEffect, useState} from "react";
import {Bar, BarChart, CartesianGrid, Legend, Rectangle, ResponsiveContainer, Tooltip, XAxis, YAxis} from "recharts";
import {
    ClientWantsToEnterDashboardDto, Devicelog,
    ServerSendsMetricToAdmin,
    StringConstants
} from "../generated-client.ts";

export default function AdminDashboard() {
    
    const {onMessage, readyState, send} = useWsClient()
    const [metric, setMetrics] = useState<Devicelog[]>([])

    useEffect(() => {
        if (readyState!=1)
            return;
        send<ClientWantsToEnterDashboardDto>({eventType: StringConstants.ClientWantsToEnterDashboardDto});
            const unsub = onMessage<ServerSendsMetricToAdmin>(StringConstants.ServerSendsMetricToAdmin, (dto) =>  {
                setMetrics(dto.metrics || []);
            })
        return () => {
                unsub();
        }
    }, [readyState]);
    
    return(<>
        <ResponsiveContainer width="100%" height="100%">
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
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="value" fill="#8884d8" activeBar={<Rectangle fill="pink" stroke="blue" />} />
            </BarChart>
        </ResponsiveContainer>
    </>)
}