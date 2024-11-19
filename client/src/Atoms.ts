import {atom} from "jotai";
import ServerSendsEchoDto from "./models/generated/ServerSendsEchoDto.ts";

export const EchoAtom = atom<ServerSendsEchoDto[]>([])