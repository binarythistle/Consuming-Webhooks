import http from 'k6/http';

export let options = {
    vus: 1,
    //duration: '10s',
    iterations: 100
};

export default function () {

    var url = "<YOUR WEBHOOK ENDPOINT HERE>";

    var payload = JSON.stringify({ name: "James Bond" });

    var params = {
        headers: {
            'Content-Type': 'application/json',
            'user-agent': 'k6-loadtest'
          
        }
    };



    var res = http.post(url, payload, params);
    console.log('Response Code was ' + String(res.status) 
        + ' / Response Time: ' + String(res.timings.duration) + ' ms'
        + ' / Response : ' + String(res.body)
    );
}


