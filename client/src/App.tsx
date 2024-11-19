
import { sendJsonMessage } from './wsclient';
import ClientWantsToEchoDto from "./models/generated/ClientWantsToEchoDto.ts";


export  const App = () => {




    const handleClickSendMessage = () => {
        var dto = {message: "hello world", eventType: "ClientWantsToEcho"};
        sendJsonMessage<ClientWantsToEchoDto>(dto);

    };


    return (
        <div>

            
        </div>)

}

export default App;