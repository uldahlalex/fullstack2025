
import useCustomWsClient from './useCustomWsClient.ts';
import ClientWantsToEchoDto from "./models/generated/ClientWantsToEchoDto.ts";
import {EchoAtom} from "./Atoms.ts";
import {useAtom} from "jotai";


export  const App = () => {

    const ws = useCustomWsClient();

const [history, setHistory] = useAtom(EchoAtom);

    const handleClickSendMessage = () => {
        var dto = {message: "hello world", eventType: "ClientWantsToEcho"};
        ws.sendJsonMessage<ClientWantsToEchoDto>(dto);

    };


    return (
        <div>
            {
                history.map((message, index) => {
                    return <div key={index}>{JSON.stringify(message)}</div>
                })
            }
            <button onClick={() => handleClickSendMessage()}>send</button>
            
        </div>)

}

export default App;