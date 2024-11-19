
import { sendJsonMessage } from './wsclient';

export interface ClientWantsToEchoDto {
    Message: string;
}

export  const App = () => {




    const handleClickSendMessage = () => {
        var dto = {Message: "hello world", eventType: "ClientWantsToEcho"};
        sendJsonMessage<ClientWantsToEchoDto>(dto);

    };


    return (
        <div>

            
        </div>)

}

export default App;