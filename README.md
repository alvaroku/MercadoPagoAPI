# MercadoPagoAPI

## Descripción

**MercadoPagoAPI** es un proyecto .NET que implementa una integración completa con Mercado Pago para la generación de preferencias de pago y la gestión de notificaciones (webhooks) con validación de `secretKey`. Además, incluye una implementación para el envío de correos electrónicos utilizando el servicio [Resend](https://resend.com/).

## Características

- **Generación de Preferencias de Pago:**  
  Permite crear preferencias de pago en Mercado Pago a través de un endpoint seguro.

- **Webhook Seguro:**  
  Recibe notificaciones de Mercado Pago y valida la autenticidad mediante una `secretKey` configurable.

- **Envío de Correos con Resend:**  
  Implementa el envío de emails transaccionales utilizando la API de Resend, ideal para notificaciones y confirmaciones.

## Endpoints Principales

- `POST /api/Checkout`  
  Genera una preferencia de pago en Mercado Pago.

- `POST /api/Checkout/webhook`  
  Recibe notificaciones de Mercado Pago y valida la `secretKey`.

- Otros endpoints auxiliares para pruebas y utilidades.

## Configuración

1. **Clonar el repositorio:**