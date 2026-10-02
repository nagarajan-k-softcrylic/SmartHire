import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface ChatHistoryItem {
  id: string;
  title: string;
}

interface FeatureCard {
  icon: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-screening',
  imports: [CommonModule, FormsModule],
  templateUrl: './screening.html',
  styleUrl: './screening.scss',
})
export class Screening {
  readonly chatHistory = signal<ChatHistoryItem[]>([]);
  readonly messageText = signal('');

  readonly featureCards: FeatureCard[] = [
    {
      icon: 'dashboard',
      title: 'Resume Screening',
      description: 'Match resumes against a job description using AI ranking.',
    },
    {
      icon: 'search',
      title: 'JD-Based Grounding',
      description: 'Ground candidate analysis in the job description context.',
    },
    {
      icon: 'candidate',
      title: 'Candidate Insights',
      description: 'Get skill gaps, highlights, and fit summaries instantly.',
    },
    {
      icon: 'report',
      title: 'Export Reports',
      description: 'Generate shortlists and export screening results.',
    },
  ];

  sendMessage(): void {
    const text = this.messageText().trim();
    if (!text) {
      return;
    }
    this.messageText.set('');
  }
}
