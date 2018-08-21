var _prevValue = null;
var _token = null;
var _identity = null;

function getCookie(){
	chrome.cookies.get({ url: 'https://warframe.market', name: 'JWT' }, cookie => {
		if (cookie && cookie.value !== _prevValue) {
			updateToken(cookie.value);
		}
		_prevValue = cookie.value;
		setTimeout(getCookie, 10000);
	});
}

function updateToken(token) {
    if (getIdentity(token) !== null) {
        _token = token;
        updateIdentity();
    }
}

function updateIdentity() {
    var newIdentity = getIdentity(_token);
    if (newIdentity !== null && newIdentity !== _identity) {
        chrome.runtime.sendNativeMessage("space.xelys.warframetradeassistanthost", { token: _token },
            result => {
                if (result) {
                    if (result.success === true) {
                        _identity = newIdentity;
                    } else {
                        console.log("Native helper reported failure");
                        setTimeout(updateIdentity, 5000);
                    }
                } else {
                    console.log(chrome.runtime.lastError);
                    setTimeout(updateIdentity, 5000);
                }
            });
    }
}

function getIdentity(token) {
    if (typeof token !== 'string') {
        return null;
    }
    var parts = token.split('.');
    if (parts.length !== 3) {
        return null;
    }
    var payload = tryDecodePayload(parts[1]);
    return payload ? (payload.jwt_identity || null) : null;
}

function tryDecodePayload(payloadString) {
    try {
        return JSON.parse(atob(payloadString.replace("_", "/").replace("-", "+") + '='.repeat((4 - payloadString.Length % 4) % 4)));
    } catch (err) {
        console.log(err);
    }
    return null;
}

getCookie();