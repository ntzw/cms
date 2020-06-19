import React, { useEffect, IframeHTMLAttributes, useState } from 'react'
import defaultConfig from '../../../config/defaultSettings'

interface EditorMdProps extends IframeHTMLAttributes<any> {
    config?: { [key: string]: any },
    onFinish?: (value: string) => void;
    onReady?: () => void;
}

export interface EditorMdInstance {
    submit: (valueType?: 'markdown' | 'html') => void;
    insertValue: (value: string) => void;
    setValue: (value: string) => void;
    setCodeMirrorOption: (mode: string) => void;
}

const EditorMdDOM: React.ForwardRefRenderFunction<EditorMdInstance, EditorMdProps> = ({ style, onReady, config, onFinish }, ref) => {
    const [editorId] = useState(`editormd${Date.now()}`);

    React.useImperativeHandle(ref, () => ({
        submit: (valueType = 'markdown') => {
            getIframeWindow(editorId)?.postMessage({
                action: valueType === 'markdown' ? 'getMarkdown' : 'getHTML',
                callback: 'getValue'
            }, '*')
        },
        insertValue: (value) => {
            getIframeWindow(editorId)?.postMessage({
                action: 'insertValue',
                data: [value]
            }, '*')
            getIframeWindow(editorId)?.postMessage({
                action: 'focus',
            }, '*')
        },
        setValue: (value) => {
            getIframeWindow(editorId)?.postMessage({
                action: 'setValue',
                data: [value]
            }, '*')
        },
        setCodeMirrorOption: (mode) => {
            getIframeWindow(editorId)?.postMessage({
                action: 'setCodeMirrorOption',
                data: ['mode', mode]
            }, '*')
        }
    }));

    function getIframeWindow(id: string) {
        const editorMdIframe = document.getElementById(id) as any;
        return editorMdIframe?.contentWindow;
    }

    const messageEvent = ({ data: { id, action, data } }: { data: { id: string, action: string; data?: any } }) => {
        if (action && editorId === id) {
            const editorMdWindow = getIframeWindow(id);
            switch (action) {
                case 'onReady':
                    editorMdWindow?.postMessage({
                        action: 'init',
                        data: {
                            ...config,
                        }
                    }, '*')
                    break;
                case 'onchange':
                    if (typeof onFinish === 'function')
                        onFinish(data);
                    break;
                case 'onload':
                    if (onReady)
                        onReady();
                    break;
                case 'getValue':
                    if (typeof onFinish === 'function')
                        onFinish(data);
                    break;
                default:
                    break;
            }
        }
    }

    useEffect(() => {
        window.addEventListener("message", messageEvent, false);

        return function cleanup() {
            window.removeEventListener("message", messageEvent, false);
        }
    });

    return <iframe
        style={style}
        frameBorder="no"
        src={`${defaultConfig.basePath}/editormd.html?id=${editorId}`}
        id={editorId}
    ></iframe>
}

const EditorMd = React.forwardRef<EditorMdInstance, EditorMdProps>(EditorMdDOM);
export default EditorMd;