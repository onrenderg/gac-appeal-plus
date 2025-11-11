var firebaseConfig = {
	apiKey: "AIzaSyAospF8JilHdE84YpzQW4oeFyhf4lOu8eY",
	authDomain: "notification-as-a-servic-f87a2.firebaseapp.com",
	projectId: "notification-as-a-servic-f87a2",
	storageBucket: "notification-as-a-servic-f87a2.appspot.com",
	messagingSenderId: "935059050895",
	appId: "1:935059050895:web:db6a67dcb5e1b88c85bbfa",
	measurementId: "G-S4EGGGGS67"
};



firebase.initializeApp(firebaseConfig);

const messaging = firebase.messaging();

function PermitUser()
{
    Notification.requestPermission().then(permission => {

        console.log(permission);


    });
   
}

function SubscribeUser() {
    Notification.requestPermission().then(permission => {
    if (permission == "granted") {
        messaging.getToken({ vapidKey: 'BLPhHpsiQsmTZ1LTFne_psVLUid8Mb2ABbmAgl17JZVt7ODbIbT98nnLeD58TAxMMO8qysbqHQaFNSzvztBoPYw' }).then(currentToken => {
            document.getElementById('token').innerHTML = currentToken;
            console.log(currentToken);

        });
    }
    });   
}

messaging.onMessage(res=>{
	console.log(res);
});