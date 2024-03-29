import IMessage from "../../../entities/IMessage";

interface MessageProps {
    message: IMessage
}

export default function Message({message}: MessageProps) {
    const win = message.text.startsWith('[win]');
    return (
        <div className="flex w-full mt-2 space-x-3 max-w-xs">
            <div className="flex-shrink-0 h-10 w-10 rounded-full bg-gray-300"></div>
            <div className="bg-blue-600 text-white p-3 rounded-r-lg rounded-bl-lg"
                 style={win ? {backgroundColor: "green"} : {}}>
                <div>
                    <p className="text-sm"><b>{message.userName}</b></p>
                    <p className="text-sm">{message.text.replace('[win] ', '')}</p>
                </div>
            </div>
        </div>
    );
}