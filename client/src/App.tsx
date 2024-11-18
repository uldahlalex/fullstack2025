import { useState, useCallback, useEffect } from 'react';
import useWebSocket, { ReadyState } from 'react-use-websocket';
import { sendJsonMessage } from './wsclient';
export interface ClientWantsToEchoDto {
    Message: string;
}

export  const App = () => {
    // const [messageHistory, setMessageHistory] = useState<MessageEvent<ClientWantsToEchoDto>[]>([]);




    const handleClickSendMessage = () => {
        var dto = {Message: "hello world", eventType: "ClientWantsToEcho"};
        sendJsonMessage<ClientWantsToEchoDto>(dto);

    };


    return (
        <div>

            
        </div>)

}

export default App;