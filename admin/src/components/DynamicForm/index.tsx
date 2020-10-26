import { DynaminFormProps, FormItem, AsyncResult, DynaminFormAction } from "./data";
import React, { useState, createRef, useEffect } from "react";
import { Spin, Input, Select, Cascader, Switch, Form, Empty } from "antd";
import { CascaderOptionType } from "antd/lib/cascader";
import { Store } from "antd/lib/form/interface";
import { FormInstance } from "antd/lib/form";
import { getFields, getAsyncData } from "./service";
import 'braft-editor/dist/index.css'
import BraftEditor, { MediaType } from 'braft-editor'
import styles from './style.less'
import UploadCustom from "../FormCustom/UploadCustom";
import defaultSetting from '../../../config/defaultSettings';
import { HandleResult } from "@/utils/request";
import { SelectProps } from "antd/lib/select";
import { EditType } from "@/pages/cms/columnlist/components/modelfieldadd";
import EditorMd from "../Editor/EditorMd";


export enum FormItemType {
    input,
    password,
    select,
    textArea,
    cascader,
    switch,
    editor,
    radio,
    checkBox,
    dataPicker,
    rangePicker,
    upload,
    region,
    tags,
}

export function GetFormItemTypeName(type: number) {
    switch (type) {
        case FormItemType.password:
            return '密码框';
        case FormItemType.select:
            return '选择器';
        case FormItemType.switch:
            return '开关';
        case FormItemType.textArea:
            return '多行文本框'
        case FormItemType.cascader:
            return '级联选择';
        case FormItemType.editor:
            return '编辑器';
        case FormItemType.radio:
            return '单选按钮';
        case FormItemType.checkBox:
            return '多选按钮';
        case FormItemType.dataPicker:
            return '日期选择框';
        case FormItemType.rangePicker:
            return '日期范围选择框';
        case FormItemType.upload:
            return '上传组件';
        case FormItemType.region:
            return '城市组件';
        case FormItemType.tags:
            return '标签';
        default:
            return '文本框';
    }
}

const editUploadFn: MediaType['uploadFn'] = param => {
    const serverURL = `${defaultSetting.basePath}/Api/Utils/Upload/BraftEditor`;
    const xhr = new XMLHttpRequest()
    const fd = new FormData()
    const successFn = () => {
        // 假设服务端直接返回文件上传后的地址
        // 上传成功后调用param.success并传入上传后的文件地址
        const res = JSON.parse(xhr.responseText) as HandleResult<string>;
        param.success({
            url: res.data || '',
            meta: {
                id: param.file.name,
                title: param.file.name,
                alt: param.file.name,
                loop: false, // 指定音视频是否循环播放
                autoPlay: false, // 指定音视频是否自动播放
                controls: true, // 指定音视频是否显示控制栏
                poster: '', // 指定视频播放器的封面
            },
        })
    }

    const progressFn = (event: any) => {
        // 上传进度发生变化时调用param.progress
        param.progress(event.loaded / event.total * 100)
    }

    const errorFn = () => {
        // 上传发生错误时调用param.error
        param.error({
            msg: 'unable to upload.',
        })
    }

    xhr.upload.addEventListener('progress', progressFn, false)
    xhr.addEventListener('load', successFn, false)
    xhr.addEventListener('error', errorFn, false)
    xhr.addEventListener('abort', errorFn, false)

    fd.append('file', param.file)
    xhr.open('POST', serverURL, true)
    xhr.send(fd)
}

/**
 * 返回表单项
 * @param item 
 */
const FormItemDOM = (item: FormItem) => {
    const type = item.type || FormItemType.input;
    switch (type) {
        case FormItemType.switch:
            return <Switch {...item.switch} />
        case FormItemType.cascader:
            return <Cascader {...item.cascader} />
        case FormItemType.textArea:
            return <Input.TextArea {...item.textarea} />
        case FormItemType.select:
            return <Select {...item.select} />;
        case FormItemType.tags:
            const tags: SelectProps<any> = {
                ...item.select,
                mode: "tags"
            }
            return <Select {...tags} />;
        case FormItemType.password:
            return <Input.Password {...item.password} />
        case FormItemType.upload:
            if (item.upload) {
                const { value, onChange, type, action, ...rest } = item.upload;
                let tempAction = action || (type === 'image' ? '/Api/Utils/Upload/Image' : '/Api/Utils/Upload/File');
                return <UploadCustom type={type || 'file'} action={tempAction} {...rest} />
            }
            return <></>;
        case FormItemType.editor:
            switch (item.editType) {
                case EditType.Markdown编辑器:
                    return <EditorMd
                        style={{ width: '100%' }}
                        config={{
                            placeholder: '输入内容。PS:只有开启实时预览，编辑的数据才会生效！！！',
                            imageUpload: true,
                            imageFormats: ["jpg", "jpeg", "gif", "png", "bmp", "webp"],
                            imageUploadURL: "/Api/Utils/Upload/EditorMd",
                            height: 540
                        }}
                    />
                default:
                    return <BraftEditor
                        className={styles.myEditor}
                        placeholder="请输入正文内容"
                        media={{
                            externals: {
                                image: false,
                                video: false,
                                audio: false,
                                embed: false,
                            },
                            uploadFn: editUploadFn,
                        }}
                    />;
            }


        default:
            return <Input {...item.input} />
    }
}

const handleCascaderValue = (options: CascaderOptionType[] | undefined, value: any): any[] => {
    var newData = [];
    if (options && value) {
        const last = (function setNewData(children): CascaderOptionType | null {
            for (let index = 0; index < children.length; index += 1) {
                const element = children[index];
                if (element.value === value) {
                    return element;
                }
                if (element.children && element.children.length > 0) {
                    const temp = setNewData(element.children);
                    if (temp) {
                        newData.unshift(temp.value);
                        return element;
                    }
                }
            }
            return null;
        })(options);
        if (last) {
            newData.unshift(last.value);
        }
    }
    return newData;
}

const DynaminForm = <T extends Store>(props: DynaminFormProps<T>) => {
    const {
        onFinish,
        fields,
        params,
        fieldActionParams,
        actionRef,
        name,
        layout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 16 },
        }
    } = props;

    const [editData, setEditData] = useState<T>();
    const [formItemData, setFormItemData] = useState<Array<FormItem>>([]);

    const [loading, setLoading] = useState({
        loadFields: false,
    });

    const [form] = useState(createRef<FormInstance>());
    const loadFieldAsyncData = (fields: FormItem[]): Promise<FormItem[]> => {
        return new Promise(resolve => {
            setFormItemData(fields);

            let asyncCount = 0;
            function asyncOver() {
                if (asyncCount === 0) {
                    setFormItemData(fields);
                    resolve(fields);
                }
            }

            function asyncData(item: FormItem) {
                if (item.dataAction) {
                    asyncCount += 1;
                    getAsyncData(item.dataAction, {
                        ...fieldActionParams && fieldActionParams(item),
                        ...item.dataParams,
                    }).then(res => {

                        switch (item.type) {
                            case FormItemType.select:
                                item.select = {
                                    ...item.select,
                                    options: res?.data || []
                                }
                                break;
                            case FormItemType.cascader:
                                item.cascader = {
                                    ...item.cascader,
                                    options: res?.data || []
                                }
                                break;
                        }

                        asyncCount -= 1;
                        asyncOver();
                    })
                }
            }

            fields.forEach(item => {
                switch (item.type) {
                    case FormItemType.select:
                    case FormItemType.cascader:
                        asyncData(item);
                        break;
                }
            })

            asyncOver();
        });
    }

    const loadFields = (url: string, params?: { [key: string]: any }) => {
        setLoading({
            ...loading,
            loadFields: true,
        })
        getFields<AsyncResult<T>>(url, params).then(res => {
            if (res?.isSuccess && res.data) {
                setEditData(res.data.editData);

                let data: any = { ...res.data.editData };
                loadFieldAsyncData(res.data.field).then(fields => {
                    form.current?.setFieldsValue(handleFormData(fields, data));
                    setLoading({
                        ...loading,
                        loadFields: false,
                    })
                });
            } else {
                setLoading({
                    ...loading,
                    loadFields: false,
                })
            }
        })
    }

    const loadAsyncField = () => {
        userAction.clear();
        if (fields instanceof Array) {
            setFormItemData(fields);
            loadFieldAsyncData(fields).then(fields => {
                setLoading({
                    ...loading,
                    loadFields: false,
                })
            });
        } else if (typeof fields === 'string' && fields) {
            loadFields(fields, params);
        }
    }

    useEffect(() => {
        loadAsyncField();
    }, [fields, JSON.stringify(params || {})]);

    const userAction: DynaminFormAction = {
        reload: () => {
            if (typeof fields === 'string' && fields) {
                loadFields(fields, params);
            }
        },
        reloadFieldItem: () => {
            loadFieldAsyncData(formItemData).then(fields => {
                setLoading({
                    ...loading,
                    loadFields: false,
                })
            });
        },
        clear: () => {
            form.current?.resetFields();
        },
        submit: () => {
            form.current?.submit();
        },
        setValue: (value) => {

            form.current?.setFieldsValue(value);
        },
        getValue: () => {
            return form.current?.getFieldsValue();
        },
        setLoading: (status) => {
            setLoading({
                ...loading,
                loadFields: status
            })
        },
        setOldValue: () => {
            userAction.setValue(handleFormData(formItemData, editData));
        }
    }

    if (actionRef && typeof actionRef === 'function') {
        actionRef(userAction);
    } else if (actionRef && typeof actionRef !== 'function') {
        actionRef.current = userAction;
    }

    return <Spin
        spinning={loading.loadFields}
        tip="表单生成中，请稍等...."
    >
        {formItemData.length > 0 ? <Form
            {...layout}
            name={name || Date.now().toString()}
            ref={form}
            onFinish={(value) => {
                if (onFinish) {
                    onFinish({
                        ...editData,
                        ...value
                    } as T)
                }
            }}
        >
            {formItemData.map(item => {
                return <Form.Item
                    key={item.name}
                    label={item.label}
                    name={item.name}
                    extra={item.extra ? <span style={{ color: 'green' }}>{item.extra}</span> : null}
                    valuePropName={item.valuePropName}
                    validateTrigger={item.validateTrigger}
                    rules={item.rules?.map((temp) => {
                        if (temp?.pattern && typeof temp.pattern === 'string') {
                            temp.pattern = new RegExp(temp.pattern);
                        }
                        return temp;
                    })}
                >
                    {FormItemDOM(item)}
                </Form.Item>
            })}
        </Form> : <Empty description="未设置表单字段" />}
    </Spin>
}

export default DynaminForm;

export const DefaultSplit = '&|&|&';

/**
 * 处理设置的数据
 * @param fields 字段
 * @param oldData 旧数据
 */
export function handleFormData(fields: FormItem[], oldData: any): any {
    const newData = { ...oldData };
    fields.forEach(field => {
        const fieldName: string = field.name;
        const oldValue: string | number = newData[fieldName];
        switch (field.type) {
            case FormItemType.cascader:
                newData[fieldName] = handleCascaderValue(field.cascader?.options, oldValue);
                break;
            case FormItemType.tags:
                setSplitValue(newData, fieldName, oldValue, field);
                break;
            case FormItemType.select:
                if (field.select) {
                    switch (field.select.mode) {
                        case 'tags':
                        case 'multiple':
                            setSplitValue(newData, fieldName, oldValue, field);
                            break;
                    }
                }
                break;
            case FormItemType.editor:
                switch (field.editType) {
                    case EditType.Markdown编辑器:
                        break;
                    default:
                        newData[fieldName] = oldValue && BraftEditor.createEditorState(oldValue);
                        break;
                }
                break;
            default:
                break;
        }
    });
    return newData;
}

function setSplitValue(newData: any, fieldName: string, oldValue: string | number, field: FormItem) {
    newData[fieldName] = [];
    if (typeof oldValue === 'string')
        newData[fieldName] = oldValue.split(field.split || DefaultSplit).filter(temp => !!temp);
}

