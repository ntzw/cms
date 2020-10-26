import React, { useEffect, IframeHTMLAttributes, useState } from 'react'
import defaultConfig from '../../../config/defaultSettings'

interface EditorMdProps extends IframeHTMLAttributes<any> {
    config?: { [key: string]: any },
    onFinish?: (value: string) => void;
    onReady?: () => void;
    value?: string;
}

export interface EditorMdInstance {
    submit: (valueType?: 'markdown' | 'html') => void;
    insertValue: (value: string) => void;
    setValue: (value: string) => void;
    setCodeMirrorOption: (mode: string) => void;
}

const EditorMdDOM: React.ForwardRefRenderFunction<EditorMdInstance, EditorMdProps> = ({ value, style, onReady, config, onFinish, onChange }, ref) => {
    const [editorId] = useState(`editormd${Date.now()}`);
    const [valueState, setValueState] = useState(true);
    const [iframeHeight, setIframeHeight] = useState((config && config.height) || 100)

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

                    if (typeof onChange === 'function')
                        onChange(data);
                    break;
                case 'onload':
                    if (onReady)
                        onReady();

                    setIframeHeight(data);
                    setEditMdValue();
                    break;
                case 'getValue':
                    if (typeof onFinish === 'function')
                        onFinish(data);
                    break;
                case 'onfullscreen':
                    break;
                case 'onfullscreenExit':
                    break;
                default:
                    break;
            }
        }
    }

    const setEditMdValue = () => {
        if (value && valueState) {
            setValueState(false);
            getIframeWindow(editorId)?.postMessage({
                action: 'setValue',
                data: [value]
            }, '*')
        }
    }

    useEffect(() => {
        window.addEventListener("message", messageEvent, false);
        return function cleanup() {
            window.removeEventListener("message", messageEvent, false);
        }
    });

    useEffect(() => {
        return () => {
            setValueState(true);
        }
    }, [])



    useEffect(() => {
        setEditMdValue();
    }, [value])

    return <iframe
        style={{
            ...style,
            height: iframeHeight
        }}
        frameBorder="no"
        src={`${defaultConfig.basePath}/editormd.html?id=${editorId}`}
        id={editorId}
    ></iframe>
}

const EditorMd = React.forwardRef<EditorMdInstance, EditorMdProps>(EditorMdDOM);
export default EditorMd;