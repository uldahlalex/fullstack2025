import {useWsClient} from "ws-request-hook";
import {ClientWantsToEchoDto, ServerSendsEchoDto, StringConstants} from '../generated-client';

export default function SignIn() {

    const { sendRequest } = useWsClient();

    const signIn = async () => {
        const signInDto: ClientWantsToEchoDto = {
            eventType: StringConstants.ClientWantsToEchoDto,
message: "hello world"
        }
        const signInResult: ServerSendsEchoDto = await sendRequest<ClientWantsToEchoDto, ServerSendsEchoDto>(signInDto, StringConstants.ServerSendsEchoDto);
        console.log(signInResult)
    };

    return (<>
        <div className="border border-red-500">auth component</div>
        <button onClick={signIn}>sign in</button>
    </>)
}