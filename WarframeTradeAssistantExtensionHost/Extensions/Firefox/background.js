var _token = null;
var _identity = null;

function subscribeToLoginCookieChange() {
    browser.cookies.onChanged.addListener(function (changeInfo) {
        var cookie = changeInfo.cookie;
        if (cookie.name === 'JWT'
            && cookie.firstPartyDomain === 'warframe.market'
            && cookie.domain === '.warframe.market'
            && !changeInfo.removed
            && changeInfo.cause === 'explicit'
        ) {
            updateToken(cookie.value);
        }
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
        browser.runtime.sendNativeMessage("space.xelys.warframetradeassistanthost", { token: _token }).then((result => {
            if (result.success === true) {
                _identity = newIdentity;
            } else {
                console.log("Native helper reported failure");
                setTimeout(updateIdentity, 5000);
            }
        }), (error) => {
            console.log(error);
            setTimeout(updateIdentity, 5000);
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

browser.cookies.get({ firstPartyDomain: 'warframe.market', url: 'https://warframe.market', name: 'JWT' }).then(cookie => {
    if (cookie) {
        updateToken(cookie.value);
    }
    subscribeToLoginCookieChange();
});