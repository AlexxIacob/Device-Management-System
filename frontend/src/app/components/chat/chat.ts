import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DeviceService } from '../../services/device.service';

interface Message {
  text: string;
  isUser: boolean;
}

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html',
  styleUrl: './chat.css'
})
export class ChatComponent {
  isOpen = false;
  messages: Message[] = [];
  inputMessage = '';
  isLoading = false;

  constructor(private deviceService: DeviceService) {}

  toggleChat(): void {
    this.isOpen = !this.isOpen;
    if (this.isOpen && this.messages.length === 0) {
      this.messages.push({
        text: 'Hello! I am your device assistant. Ask me anything about devices!',
        isUser: false
      });
    }
  }

  sendMessage(): void {
    if (!this.inputMessage.trim() || this.isLoading) return;

    const userMessage = this.inputMessage.trim();
    this.messages.push({ text: userMessage, isUser: true });
    this.inputMessage = '';
    this.isLoading = true;

    this.deviceService.chat(userMessage).subscribe({
      next: (response) => {
        this.messages.push({ text: response.reply, isUser: false });
        this.isLoading = false;
      },
      error: () => {
        this.messages.push({ text: 'Sorry, I could not process your request.', isUser: false });
        this.isLoading = false;
      }
    });
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }
}