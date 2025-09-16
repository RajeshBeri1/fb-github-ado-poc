import { createComponent } from '@lit/react';
import React from 'react';
import { OmniModal } from 'omni-ui/omni-modal.js';


export const Modal = createComponent({
    tagName: 'omni-modal',
    elementClass: OmniModal,
    react: React,
    events: {
        showModal: "showModal",
        onCancel: "cancel",
        onclose: "close",
    }
});
