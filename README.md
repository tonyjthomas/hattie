# Home Assistant Terminal Text Interface Executor

This is an overkill wrapper around Home Assistant's conversation/process REST API endpoint. Its point is to enable control of devices just by typing conversationally into the terminal.

As it is just a wrapper, Hattie's effectiveness is entirely dependent on your [Conversation Agent](https://www.home-assistant.io/integrations/conversation/) setup. If it's not working well, you may need to add custom sentences or configure an entirely new agent in your Home Assistant. 

## Configuration

Create a file called `.hattie.json` in your home directory that looks like this:
```json
{
    "url": "your-home-assistant-instance-address",
    "token": "your-long-lived-access-token"
}
```

## Usage

Once you've built a binary for your machine and added it to your path, just run it and supply your text prompt.
E.g.
```
> hattie what time is it
3:52 PM
> hattie turn off the lights in the kitchen
Turned off the lights
```

## FAQ

### Why is the JSON handling so weird?
Using System.Text.Json serialization requires reflection (at least until [this changes](https://github.com/fsharp/fslang-suggestions/issues/864)), which breaks AOT compilation. 

### This is dumb, I can just use bash.
Not a question, but yes. Here's some bash that will accomplish the same thing:
```bash
> curl -s -H "Authorization: Bearer your-long-lived-access-token" your-home-assistant-instance-address/api/conversation/process -d "{\"text\": \"what time is it\" }" | jq -r .response.speech.plain.speech
```

### This is dumb, I can just use pwsh.
Also not a question, but yes. Here's some powershell that will accomplish the same thing:
```pwsh
> (irm your-home-assistant-address/api/conversation/process -me POST -au Bearer -to (write your-long-lived-access-token | ConvertTo-SecureString -AsPlainText) -b (@{text="what time is it"} | ConvertTo-Json)).response.speech.plain.speech
```