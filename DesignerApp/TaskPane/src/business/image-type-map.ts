import { ImageType } from '@omniflow/omni-webapi';

export const ImageTypeMap = (type: string): ImageType => {
    if (type.includes('png')) {
        return ImageType.PNG;
    }
    if (type.includes('gif')) {
        return ImageType.GIF;
    }
    if (type.includes('bmp') || type.includes('dib')) {
        return ImageType.BMP;
    }
    if (type.includes('tif')) {
        return ImageType.TIFF;
    }

    return ImageType.JPEG;
};
