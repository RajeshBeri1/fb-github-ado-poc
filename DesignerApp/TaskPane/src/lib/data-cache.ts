import { dataApi } from '../lib/api';

type DataRequest = any;

const _inflightDataRequests: Map<string, Promise<any>> = new Map();

export async function getDataCached(body: DataRequest) {
    const key = JSON.stringify(body || {});
    if (_inflightDataRequests.has(key)) {
        return _inflightDataRequests.get(key);
    }

    const p = (async () => {
        try {
            return await dataApi.dataGetData(body);
        } finally {
            // cleanup so we don't hold memory indefinitely
            _inflightDataRequests.delete(key);
        }
    })();

    _inflightDataRequests.set(key, p);
    return p;
}

export function clearDataCache() {
    _inflightDataRequests.clear();
}
