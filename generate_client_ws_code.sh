pnpm i -g @asyncapi/cli
asyncapi generate fromTemplate http://localhost:5000/asyncapi/asyncapi.json @asyncapi/html-template@0.20.0 -o client/src/models/generated/