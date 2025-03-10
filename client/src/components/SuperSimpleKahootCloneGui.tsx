import {useWsClient} from "ws-request-hook";
import {useEffect} from "react";

interface 

export default function SuperSimpleKahootCloneGui() {
    
    const {readyState, onMessage} = useWsClient();

    useEffect(() => {
        onMessage<>
    }, []);
    
    return (<>
    
    
    </>)
} 