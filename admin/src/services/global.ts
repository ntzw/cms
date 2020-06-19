import request from '@/utils/request';
import defaultConfig from '../../config/defaultSettings'

export async function postAjax(url: string, params: { [key: string]: any }) {
    return request(defaultConfig.basePath + url, {
        method: 'POST',
        data: params,
    });
}