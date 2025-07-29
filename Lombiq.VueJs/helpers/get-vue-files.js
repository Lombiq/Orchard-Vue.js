import { readdirSync } from 'fs';

const asDirent = { withFileTypes: true };

export function getVueComponents(rootPath)
{
    const filter = (dirent) => dirent.name.endsWith('.vue') && dirent.isFile();
    return readdirSync(rootPath, asDirent).filter(filter).map((dirent) => dirent.name);
}
