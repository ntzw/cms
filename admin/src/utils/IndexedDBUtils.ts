
const openDbBase = (databaseName: string, v?: number) => {
    return new Promise<IDBDatabase>(resolve => {
        const request = window.indexedDB.open(databaseName, v);
        request.onerror = (event) => {
            console.error('本地数据库打开出错', event);
        }

        request.onsuccess = ({ target }: { target: any }) => {
            resolve(target.result);
        }

        request.onupgradeneeded = ({ target }: { target: any }) => {
            resolve(target.result);
        }
    })
}

export default function (databaseName: string) {
    return {
        createTable: (tableName: string, optionalParameters?: IDBObjectStoreParameters) => {
            return new Promise<IDBObjectStore>(resolve => {
                openDbBase(databaseName).then(db => {
                    let dbStore;
                    if (!db.objectStoreNames.contains(tableName)) {
                        const newV = db.version + 1;
                        db.close();
                        openDbBase(databaseName, newV).then(db => {
                            let dbStore;
                            if (!db.objectStoreNames.contains(tableName)) {
                                dbStore = db.createObjectStore(tableName, optionalParameters || { autoIncrement: true });
                            } else {
                                dbStore = db.transaction([tableName], 'readwrite').objectStore(tableName);
                            }
                            resolve(dbStore);
                        })
                    } else {
                        dbStore = db.transaction([tableName], 'readwrite').objectStore(tableName);
                        resolve(dbStore);
                    }
                })
            })
        },
        append: (tableName: string, data: { [key: string]: any }) => {
            return new Promise(resolve => {
                openDbBase(databaseName).then(db => {
                    const dbStore = db.transaction([tableName], 'readwrite').objectStore(tableName);
                    if (dbStore) {
                        const res = dbStore.add(data);
                        res.onsuccess = () => {
                            resolve();
                            db.close();
                        }

                        res.onerror = (event) => {
                            console.error('数据写入失败', event);
                        }
                    }
                })
            })
        },
        getAll: <T>(tableName: string) => {
            return new Promise<T[]>(resolve => {
                openDbBase(databaseName).then(db => {
                    const dbStore = db.transaction(tableName).objectStore(tableName);
                    if (dbStore) {
                        const res = dbStore.openCursor();
                        const data: T[] = [];
                        res.onsuccess = ({ target }: { target: any }) => {
                            var cursor = target?.result as any;
                            if (cursor) {
                                data.push(cursor.value);
                                cursor.continue();
                            } else {
                                resolve(data);
                                db.close();
                            }
                        }
                    }
                })
            })
        },
        get: <T>(tableName: string, key: any) => {
            return new Promise<T>(resolve => {
                openDbBase(databaseName).then(db => {
                    const dbStore = db.transaction(tableName).objectStore(tableName);
                    if (dbStore) {
                        const res = dbStore.get(key);
                        res.onsuccess = () => {
                            resolve(res.result);
                            db.close();
                        }
                    }
                })
            })
        },
        update: (tableName: string, data: any) => {
            return new Promise(resolve => {
                openDbBase(databaseName).then(db => {
                    const dbStore = db.transaction(tableName, 'readwrite').objectStore(tableName);
                    if (dbStore) {
                        const res = dbStore.put(data);
                        res.onsuccess = () => {
                            resolve();
                            db.close();
                        }
                    }
                })
            })
        },
        delete: (tableName: string, key: any) => {
            return new Promise(resolve => {
                openDbBase(databaseName).then(db => {
                    const dbStore = db.transaction(tableName, 'readwrite').objectStore(tableName);
                    if (dbStore) {
                        const res = dbStore.delete(key);
                        res.onsuccess = () => {
                            resolve();
                            db.close();
                        }
                    }
                })
            })
        }
    }
}