import { useState, useCallback, useEffect } from 'react';
import useWebSocket, { ReadyState } from 'react-use-websocket';
export interface ClientWantsToEchoDto {
    Message: string;
}

export  const App = () => {
    const [socketUrl, setSocketUrl] = useState('ws://localhost:8181');
    const [messageHistory, setMessageHistory] = useState<MessageEvent<ClientWantsToEchoDto>[]>([]);

    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);

    useEffect(() => {
        if (lastMessage !== null) {
            setMessageHistory((prev) => prev.concat(lastMessage));
        }
    }, [lastMessage]);

    const handleClickChangeSocketUrl = useCallback(
        () => setSocketUrl('ws://localhost:8181'),
        []
    );

    const handleClickSendMessage = useCallback(() => sendMessage(JSON.stringify({message: "hello world", eventType: "ClientWantsToEcho"})), []);

    const connectionStatus = {
        [ReadyState.CONNECTING]: 'Connecting',
        [ReadyState.OPEN]: 'Open',
        [ReadyState.CLOSING]: 'Closing',
        [ReadyState.CLOSED]: 'Closed',
        [ReadyState.UNINSTANTIATED]: 'Uninstantiated',
    }[readyState];

    return (
        <div>
            <button onClick={handleClickChangeSocketUrl}>
                Click Me to change Socket Url
            </button>
            <button
                onClick={handleClickSendMessage}
                disabled={readyState !== ReadyState.OPEN}
            >
                Click Me to send 'Hello'
            </button>
            <span>The WebSocket is currently {connectionStatus}</span>
            {lastMessage ? <span>Last message: {lastMessage.data}</span> : null}
            <ul>
                {messageHistory.map((message, idx) => (
                    <span key={idx}>{message ? message.data.Message : null}</span>
                ))}
            </ul>
        </div>
    );
};
export default App;