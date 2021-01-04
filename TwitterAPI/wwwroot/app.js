// Tim Conrad
// Copyright 2021
// MIT License

// Dependency: jQuery
// Dependency: Html Ids below must match Ids in Html file
// UI elements
const connectButton = document.getElementById("connectButton");
const stateLabel = document.getElementById("stateLabel");
const commsLog = document.getElementById("commsLog");
const closeButton = document.getElementById("closeButton");

// Stat elements
const stats = document.getElementById("stats");
const total = document.getElementById("stats-total");
const begin = document.getElementById("stats-begin");
const end = document.getElementById("stats-end");
const uptime = document.getElementById("stats-uptime");
const emojis = document.getElementById("stats-emojis");
const hashtags = document.getElementById("stats-hashtags");
const urls = document.getElementById("stats-urls");

// media
const mediaTotal = document.getElementById("stats-media-total");
const mediaImages = document.getElementById("stats-media-images");
const mediaGIFs = document.getElementById("stats-media-gifs");
const mediaVideos = document.getElementById("stats-media-videos");

// rates
const rateHour = document.getElementById("stats-rate-hour");
const rateMinute = document.getElementById("stats-rate-minute");
const rateSecond = document.getElementById("stats-rate-second");

// tops
const topHashtags = document.getElementById("top-hashtags");
const topEmojis = document.getElementById("top-emojis");
const topDomains = document.getElementById("top-domains");

// internal
var socket;
const scheme = document.location.protocol === "https:" ? "wss" : "ws";
const port = document.location.port ? (":" + document.location.port) : "";
const connectionUrl = scheme + "://" + document.location.hostname + port + "/ws";


// remove jquery?
$(document).ready(function () {
    bind();

    connect();
});

function updateState() {
    function disable() {
        closeButton.disabled = true;
    }
    function enable() {
        closeButton.disabled = false;
    }

    connectButton.disabled = true;

    if (!socket) {
        disable();
    } else {
        switch (socket.readyState) {
            case WebSocket.CLOSED:
                stateLabel.innerHTML = "Closed";
                disable();
                connectionUrl.disabled = false;
                connectButton.disabled = false;
                break;
            case WebSocket.CLOSING:
                stateLabel.innerHTML = "Closing...";
                disable();
                break;
            case WebSocket.CONNECTING:
                stateLabel.innerHTML = "Connecting...";
                disable();
                break;
            case WebSocket.OPEN:
                stateLabel.innerHTML = "Open";
                enable();
                break;
            default:
                stateLabel.innerHTML = "Unknown WebSocket State: " + htmlEscape(socket.readyState);
                disable();
                break;
        }
    }
}

function bind() {
    closeButton.onclick = close;

    connectButton.onclick = connect;
}

function connect() {
    if (socket?.readyState === WebSocket.OPEN) {
        alert("socket already connected");
        return;
    }

    stateLabel.innerHTML = "Connecting";
    socket = new WebSocket(connectionUrl);
    socket.onopen = function (event) {
        updateState();
        commsLog.innerHTML += '<tr>' +
            '<td colspan="3" class="commslog-data">Connection opened</td>' +
            '</tr>';
    };
    socket.onclose = function (event) {
        updateState();
        commsLog.innerHTML += '<tr>' +
            '<td colspan="3" class="commslog-data">Connection closed. Code: ' + htmlEscape(event.code) + '. Reason: ' + htmlEscape(event.reason) + '</td>' +
            '</tr>';
    };
    socket.onerror = updateState;
    socket.onmessage = function (event) {
        updateStats(event.data);
    };
}

function updateStats(data) {
    let stats = JSON.parse(data);

    // counts
    total.innerHTML = stats.Total;
    emojis.innerHTML = stats.Emojis + ` (${stats.EmojiPct})`; 
    hashtags.innerHTML = stats.Hashtags + ` (${stats.HashtagPct})`;
    urls.innerHTML = stats.Urls + ` (${stats.UrlPct})`;
    mediaTotal.innerHTML = stats.Media + ` (${stats.MediaPct})`;
    mediaImages.innerHTML = stats.ImagesPct;    //stats.Images + ` (${stats.ImagesPct})`;
    mediaGIFs.innerHTML = stats.GIFsPct;
    mediaVideos.innerHTML = stats.VideosPct;

    // rates
    rateHour.innerHTML = stats.RateHour;
    rateMinute.innerHTML = stats.RateMinute;
    rateSecond.innerHTML = stats.RateSecond;

    // top value arrays
    topEmojis.innerHTML = formatArray(stats.TopEmojis);
    topHashtags.innerHTML = formatArray(stats.TopHashtags);
    topDomains.innerHTML = formatArray(stats.TopDomains);

    // dates
    let beginDate = new Date(stats.Begin);
    begin.innerHTML = formatTime(beginDate);

    let endDate = new Date(stats.End);
    end.innerHTML = formatTime(endDate);

    // timespan string
    uptime.innerHTML = stats.Uptime;
}

function close() {
    if (!socket || socket.readyState !== WebSocket.OPEN) {
        alert("socket not connected");
    }
    socket.close(1000, "Closing from client");
}


function htmlEscape(str) {
    return str.toString()
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}

function formatArray(array) {
    if (!array)
        return "";

    let output = "";
    let n = 1;

    array.forEach(function (i) {
        output += n++ + ".&nbsp;" + i + "<br/>";
    });

    return output;
}

function formatDate(date) {
    return date.toLocaleDateString("en-US");
}

function formatTime(date) {
    return date.toLocaleString();
}
