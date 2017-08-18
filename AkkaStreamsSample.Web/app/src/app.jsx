import React from 'react';
import update from 'immutability-helper';

export default class App extends React.Component {
    
    constructor() {
        super();

        this.state = {
            connectionState: 'disconnected',
            msg: {}
        }
    }

    componentDidMount() {
        const hub = $.connection.applicationHub;

        hub.client.sendMessage = msg => {
            this.setState(previousState => update(previousState, {
                msg: {
                    id: { $set: msg.Id },
                    text: {$set: msg.Text }
                }
            }))

            this.setState({
                msg: {
                    id: msg.Id,
                    text: msg.Text
                }
            });
        };

        $.connection.hub.start()
           .done(_ => { 
               console.log('signalr connected, connection id ' + $.connection.hub.id);
               
               this.setState(previousState => update(previousState, { 
                   connectionState: {$set: 'connected'}
               }));
           })
           .fail(_ => { 
               console.log('signalr failed to connect');
               
               this.setState(previousState => update(previousState, {
                   connectionState: {$set: 'disconnected'}
               }));
           });
    }

    render() {
        return (
            <div className="App">
                <h1>Connection status: {this.state.connectionState}</h1>
                {this.state.msg.id && 
                    <p>Last message: #{this.state.msg.id}: {this.state.msg.text}</p>
                }
            </div>
        );
    }
}