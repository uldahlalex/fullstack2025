import { useEffect } from "react";
import useWebSocket, {ReadyState} from "react-use-websocket";

export const { lastMessage, sendJsonMessage,  readyState } = useWebSocket('ws://localhost:8181');

export const connectionStatus = {
    [ReadyState.CONNECTING]: 'Connecting',
    [ReadyState.OPEN]: 'Open',
    [ReadyState.CLOSING]: 'Closing',
    [ReadyState.CLOSED]: 'Closed',
    [ReadyState.UNINSTANTIATED]: 'Uninstantiated',
}[readyState];

useEffect(() => {
    if (lastMessage !== null) {
        // setMessageHistory((prev) => prev.concat(lastMessage));
    }
}, [lastMessage]);
