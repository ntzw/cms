import request from '@/utils/request';

export async function postAjax(url: string, params: { [key: string]: any }) {
    return request(url, {
        method: 'POST',
        data: params,
    });
}